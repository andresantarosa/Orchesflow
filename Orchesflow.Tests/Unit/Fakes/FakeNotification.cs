using MediatR;

namespace Orchesflow.Tests.Unit.Fakes
{
    public class FakeNotification : INotification
    {
        public FakeNotification(int key, string value)
        {
            Key = key;
            Value = value;
        }

        public int Key { get; set; }
        public string Value { get; set; }
    }
}