using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using WebApplication1;

[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Service : IService
{
    /// <summary>
    /// This service is a single session service due to the use of Singelton pattern.
    /// For multiple sessions/Clients singleton classes must be rewritten and sessions used insted
    /// placing all information Memorymapped based on Client session will solve this.
    /// </summary>
    public void KeyBoardHandler(int rawPackets)
    {
        var instance = KeyboardHandler.Instance;
        instance.Queue(rawPackets);
    }
    public void MouseCoords(int[] rawPackets)
    {
        var instance = MouseHandler.Instance;
        instance.SetCoordinates(rawPackets);
    }
    public void StartClient(int rawPackets)
    {
        var instance = StartClientStreamCS.Instance;
        var instancem = WriteMMF.Instance; // Just initilize the Memory mapped file writer so the reader don't need exceptionhandling 
        instance.StartClientStream = rawPackets;
    }

    public void MouseEvents(int rawPackets)
    {
        var instance = MouseHandler.Instance;
        instance.Queue(rawPackets);
    }
    public int? GetKeyBordEvent()
    {
        var instance = KeyboardHandler.Instance;
        return instance.Dequeue();
    }
    public void SetData(byte[] capture)
    {
        var instance = WriteMMF.Instance;
        var newSize = ReadMMF.Instance;
        newSize.BytesToRead(capture.Length);
        instance.WriteMemoryMappedFile(capture);
    }

    /// <summary>
    /// I'm getting the image in base64 encoded format, just for simplicity.
    /// It would be a lot better performance to use a blob instead, but for this little hobby project it will do just fine :) 
    /// </summary>
    /// <returns>base64 image</returns>
    public byte[] GetBase64Img()
    {
        WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
        var instance = ReadMMF.Instance;
        return instance.ReadMemoryMappedFile();
    }


    public int[] GetMouseCoords()
    {
        var instance = MouseHandler.Instance;
        return instance.GetCoordinates();
         
    }

    public int? GetMouseEvents()
    {
        var instance = MouseHandler.Instance;
        return instance.Dequeue();
    }


    public int? GetStartEvent()
    {
        var instance = StartClientStreamCS.Instance;
       return instance.StartClientStream;
    }
}


