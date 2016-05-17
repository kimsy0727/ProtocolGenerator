using System;
using Common;
using System.Data;

namespace ProtocolGenerator
{
    public class ProtocolManager
    {
        private Int16 mType;
        private String mXmlPath;
        private String mHeaderFilePath;
        private String mSourceFilePath;

        private ProtocolGenerator mGenerator;
        private XMLManager mXmlManager;
        private FileManager mFileManager;

        public ProtocolManager(Int16 type, String xmlPath, String filePath)
        {
            mType = type;
            mXmlPath = xmlPath;

            // xml �����͸� dataSet ���·� �����´�.
            mXmlManager = new XMLManager(mXmlPath);
            DataSet dataSet = mXmlManager.Load();
            if (dataSet == null)
            {
                Console.WriteLine("XMLManager::Load() return false. " + mXmlPath);
            }

            if (mType == (Int16)Common.ProtocolType.CPP)
            {
                mGenerator = new CPPGenerator(dataSet);
                mHeaderFilePath = filePath + "\\ProtocolHandler.h";
                mSourceFilePath = filePath + "\\ProtocolHandler.cpp";
            }
            else if (mType == (Int16)Common.ProtocolType.CS)
            {
                mGenerator = new CSGenerator(dataSet);
                mHeaderFilePath = "";
                mSourceFilePath = filePath + "\\ProtocolHandler.cs"; 
            }
            else if(mType == (Int16)Common.ProtocolType.CSWEB)
            {
                mGenerator = new CSWebGenerator(dataSet);
                mHeaderFilePath = "";
                mSourceFilePath = filePath + "\\ProtocolHandler.cs";
            }
        }
        public Boolean Execute()
        {
            try
            {

                String headerComment = "";
                String sourceComment = "";
                // �ڵ� ���� ��ü�� ���� ���ڿ� ����
                if (mGenerator.Generate(ref headerComment, ref sourceComment) != true)
                {
                    Console.WriteLine("ProtocolGenerater::Generate() return false.");
                    return false;
                }

                // ������ ���ڿ��� ���Ͽ� ����
                mFileManager = new FileManager(mHeaderFilePath);
                if (mHeaderFilePath != "" && mFileManager.Write(headerComment) != true)
                {
                    Console.WriteLine("FileManager::Write() return false.");
                    return false;
                }

                mFileManager = new FileManager(mSourceFilePath);
                if (mSourceFilePath != "" && mFileManager.Write(sourceComment) != true)
                {
                    Console.WriteLine("FileManager::Write() return false.");
                    return false;
                }

                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}