namespace GH.MongoDb.Interfaces
{
    public interface IDocument<T>
    {
        T Id { get; set; }
    }
}