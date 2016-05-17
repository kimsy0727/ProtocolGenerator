using System;
using System.Text;

namespace Common
{
    public class FileManager {
	    private String mName ;

	    public FileManager(String name)
        {
            mName = name;
	    }
        public Boolean AppendLog(String comment)
        {
            try
            {
                System.IO.File.AppendAllText(mName, "["+System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"]:"+comment+"\n");
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
        }
	    public Boolean Write(String comment)
        {
            try
            {
                System.IO.File.WriteAllText(mName, comment, Encoding.Unicode);
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
	    }
    }
}

