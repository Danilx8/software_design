namespace Software_Design._12._Sudden_Changes;

/**
 * Код ниже - 3 метода отправки сообщения по соответствующей сессии. В сессии хранятся определённые данные.
 * Сообщения есть разных типов, и именно эти отправляются запланированно (с некоторой задержкой)
 * Объединив их единым родительским классом, можно сделать их запланированную отправку отдельным меотдом
 * 
 *private static async executeGreeting(message: ScheduledMessage, session: AutomationSession): Promise<void> {
    try {
      if (session.status !== AutomationStatus.GREETING) {
        return;
      }

      const dialog = await DialogService.getDialogById(session.dialogId);
      if (!dialog || !dialog.isAutomatic) {
        logger.info(`Greeting not sent for session ${session.id}: isAutomatic is false`);
        return;
      }

      const greetingMessage = await this.getGreetingMessage(session.userId, message.messageIndex);
      if (!greetingMessage) {
        return;
      }

      await this.sendMessage(
        message.workerId,
        session.dialogId,
        session.itemId,
        session.itemName,
        greetingMessage.text,
        session.userId,
        false
      );

      logger.info(`Sent greeting message for session ${session.id} (count: ${message.messageIndex})`);
    } catch (error) {
      logger.error(`Error executing greeting for session ${session.id}:`, error);
    }
  }

  private static async executeAutoReply(message: ScheduledMessage, session: AutomationSession): Promise<void> {
    try {
      if (session.status !== AutomationStatus.RESPONDING) {
        return;
      }

      const dialog = await DialogService.getDialogById(session.dialogId);
      if (!dialog || !dialog.isAutomatic) {
        logger.info(`Auto reply not sent for session ${session.id}: isAutomatic is false`);
        return;
      }

      const replyMessage = await this.getAutoReplyMessage(session.userId, message.messageIndex);
      if (!replyMessage) {
        return;
      }

      await this.sendMessage(
        message.workerId,
        session.dialogId,
        session.itemId,
        session.itemName,
        replyMessage.text,
        session.userId,
        replyMessage.isSentWithQr
      );

      // Отправляем все последующие сообщения с isSentImmediately
      let currentIndex = message.messageIndex + 1;
      let nextMessage = await this.getAutoReplyMessage(session.userId, currentIndex);

      while (nextMessage && nextMessage.isSentImmediately) {
        await this.sendMessage(
          message.workerId,
          session.dialogId,
          session.itemId,
          session.itemName,
          nextMessage.text,
          session.userId,
          nextMessage.isSentWithQr
        );

        currentIndex++;
        nextMessage = await this.getAutoReplyMessage(session.userId, currentIndex);
      }

      // Обновляем replyIndex до последнего отправленного сообщения
      session.replyIndex = currentIndex;

      await RedisService.saveAutomationSession(session.id, session);
      logger.info(`Sent auto reply for session ${session.id}`);
    } catch (error) {
      logger.error(`Error executing auto reply for session ${session.id}:`, error);
    }
  }

  private static async executeMailMessage(session: AutomationSession): Promise<void> {
    const dialog = await DialogService.getDialogById(session.dialogId);
    if (!dialog || !dialog.isAutomatic) {
      logger.info(`Auto reply not sent for session ${session.id}: isAutomatic is false`);
      return;
    }

    const mailResult = await TemplateService.getMailTemplateWithIndex(session.userId);
    if (!mailResult) throw new Error("Could not find mail template");
    const { message, index } = mailResult;

    await this.sendMessage(
      session.workerId,
      session.dialogId,
      session.itemId,
      session.itemName,
      message.text,
      session.userId,
      false
    );

    let currentIndex = index + 1;
    let nextMessage = await this.getAutoReplyMessage(session.userId, currentIndex);

    while (nextMessage && nextMessage.isSentImmediately) {
      await this.sendMessage(
        session.workerId,
        session.dialogId,
        session.itemId,
        session.itemName,
        nextMessage.text,
        session.userId,
        nextMessage.isSentWithQr
      );

      currentIndex++;
      nextMessage = await this.getAutoReplyMessage(session.userId, currentIndex);
    }

    // Обновляем replyIndex до последнего отправленного сообщения
    session.replyIndex = currentIndex;
    await RedisService.saveAutomationSession(session.id, session);
    logger.info(`Sent mail reply for session ${session.id}`);
  }
 * 
 */

public abstract class ScheduledMessage
{
  public string text;
  public DateTime createdAt;
  
  protected ScheduledMessage(string text, DateTime createdAt)
  {
    this.text = text;
    this.createdAt = createdAt;
  }

  public string TransformText()
  {
    return text;
  }

  public abstract Task ExecuteAsync(AutomationSession session);
}

public class GreetingMessage : ScheduledMessage
{
  public int messageIndex;

  public GreetingMessage(string text, DateTime createdAt, int messageIndex) : base(text, createdAt)
  {
    this.messageIndex = messageIndex;
  }

  public override async Task ExecuteAsync(AutomationSession session)
  {
    try
    {
      if (session.status != AutomationStatus.GREETING)
      {
        return;
      }

      var dialog = await DialogService.GetDialogByIdAsync(session.dialogId);
      if (dialog == null || !dialog.isAutomatic)
      {
        return;
      }

      var greetingMessage = await GetGreetingMessageAsync(session.userId, messageIndex);
      if (greetingMessage == null)
      {
        return;
      }

      // Отправка сообщения
    }
    catch (Exception error)
    {
    }
  }

  private async Task<GreetingMessage> GetGreetingMessageAsync(int userId, int index)
  {
    // Реализация получения приветственного сообщения
    throw new NotImplementedException();
  }
}

public class AutoReplyMessage(
  string text,
  DateTime createdAt,
  int messageIndex,
  bool isSentWithQr = false,
  bool isSentImmediately = false)
  : ScheduledMessage(text, createdAt)
{
  public readonly int messageIndex = messageIndex;
  public bool isSentWithQr = isSentWithQr;
  public bool isSentImmediately = isSentImmediately;

  public override async Task ExecuteAsync(AutomationSession session)
  {
    try
    {
      if (session.status != AutomationStatus.RESPONDING)
      {
        return;
      }

      var dialog = await DialogService.GetDialogByIdAsync(session.dialogId);
      if (dialog == null || !dialog.isAutomatic)
      {
        return;
      }

      var replyMessage = await GetAutoReplyMessageAsync(session.userId, messageIndex);
      if (replyMessage == null)
      {
        return;
      }

      // Отправка сообщения
      
      // Отправляем все последующие сообщения с isSentImmediately
      int currentIndex = messageIndex + 1;
      var nextMessage = await GetAutoReplyMessageAsync(session.userId, currentIndex);

      while (nextMessage != null && nextMessage.isSentImmediately)
      {
        // Отправка сообщения
      }
    }
    catch (Exception error)
    {
    }
  }

  private async Task<AutoReplyMessage> GetAutoReplyMessageAsync(int userId, int index)
  {
    // Реализация получения автоответа
    throw new NotImplementedException();
  }
}

public class MailMessage : ScheduledMessage
{
  public MailMessage(string text, DateTime createdAt) : base(text, createdAt)
  {
  }

  public override async Task ExecuteAsync(AutomationSession session)
  {

    var mailResult = await TemplateService.GetMailTemplateWithIndexAsync(session.userId);
    if (mailResult == null)
    {
      throw new Exception("Could not find mail template");
    }

    // Отправка сообщения

    int currentIndex = mailResult.index + 1;
    var nextMessage = await GetAutoReplyMessageAsync(session.userId, currentIndex);

    while (nextMessage != null && nextMessage.isSentImmediately)
    {
      // Отправка сообщения
    }

    session.replyIndex = currentIndex;
  }

  private async Task<AutoReplyMessage> GetAutoReplyMessageAsync(int userId, int index)
  {
    // Реализация получения автоответа
    throw new NotImplementedException();
  }
}


public class AutomationSession
{
  public string id;
  public int dialogId;
  public int workerId;
  public int itemId;
  public string itemName;
  public int userId;
  public AutomationStatus status;
  public int replyIndex;
}

public enum AutomationStatus
{
  GREETING,
  RESPONDING
}

public static class DialogService
{
  public static Task<Dialog> GetDialogByIdAsync(int dialogId) => throw new NotImplementedException();
}

public class Dialog
{
  public bool isAutomatic;
}

// Благодаря этому не придётся каждый раз переписывать метод отправки запланированного сообщения
private static async Task ExecuteScheduledMessageAsync(ScheduledMessage message, AutomationSession session)
{
  await message.ExecuteAsync(session);
}
