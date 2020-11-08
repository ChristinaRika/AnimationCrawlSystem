using System.Collections.Generic;

public class LoginMsg{
    //from server.
    //0:fail.  1:succ
    public int code = 0;
    //server return message
    public string msg;
}
public class RegisterMsg{
    //from server.
    //0:fail.  1:succ
    public int code = 0;
    //server return message
    public string msg;
}
public class ImgUnit{
    public string file_name;
    public int file_size;
}

public class ImgReceive{
    public List<ImgUnit> lists = new List<ImgUnit>();
}