using System;
namespace TheQTablet.Core.Rest.Interfaces
{
    public interface IQSimClient
    {
        string Host { get; set; }
        string BaseURL { get; }
    }
}
