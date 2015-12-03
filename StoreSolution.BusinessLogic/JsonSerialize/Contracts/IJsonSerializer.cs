namespace StoreSolution.BusinessLogic.JsonSerialize.Contracts
{
    public interface IJsonSerializer
    {
        string Serialize(object inputObject);

        T Deserialize<T>(string inputString);
    }
}