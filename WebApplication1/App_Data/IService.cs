using System.ServiceModel;
using System.ServiceModel.Web;

// NOTE: If you change the interface name "IService" here, you must also update the reference to "IService" in Web.config.
[ServiceContract]
public interface IService
{

    #region Ajax Methods
    [OperationContract]
    [WebInvoke(Method = "POST",RequestFormat = WebMessageFormat.Json,BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    void KeyBoardHandler(int rawPackets);

    [OperationContract]
    [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    void StartClient(int rawPackets);

    [OperationContract]
    [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    void MouseCoords(int[] rawPackets);

    [OperationContract]
    [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    void MouseEvents(int rawPackets);

    [OperationContract]
    [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare)]
    byte[] GetBase64Img();

#endregion
    #region Wcf Methods
    [OperationContract]
    void SetData(byte[] capture);

    [OperationContract]
    int? GetKeyBordEvent();

    [OperationContract]
    int? GetStartEvent();

    [OperationContract]
    int[] GetMouseCoords();

    [OperationContract]
    int? GetMouseEvents();
    #endregion
}
