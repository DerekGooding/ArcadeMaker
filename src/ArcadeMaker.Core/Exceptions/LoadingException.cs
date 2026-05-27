namespace ArcadeMaker.Core.Exceptions
{
    public class LoadingException(string msg, Exception? innerEx = null) : Exception(msg, innerEx);
}