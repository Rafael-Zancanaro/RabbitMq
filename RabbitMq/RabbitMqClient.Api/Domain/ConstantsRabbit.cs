namespace RabbitMqClient.Send.Domain
{
    public class ConstantsRabbit
    {
        public const string NameExchangeDeadLetter = NameExchange + ".DeadLetters";
        public const string NameQueueDeadLetter = NameQueue + ".DeadLetters";
        public const string NameExchange = "NameExchange";
        public const string NameQueue = "NameQueue";
    }
}
