using System;
using System.Data;
using System.Collections.Generic;
using Common;

namespace ProtocolGenerator
{
    public class CPPGenerator : ProtocolGenerator
    {
        public CPPGenerator(DataSet dataSet) : base(dataSet)
        {

        }
        protected override String GetOpeningForHeader()
        {
            return "#include \"../CommmonCPP/" + mPeerName + ".h\"\n" +
                "#include \"../CommmonCPP/" + mBufferClassName + ".h\"\n\nclass " + mClassName + "\n" +
                "{\n\t" + mPeerName + "* mpPeer;\npublic:\n\t" +
                mClassName + "("+mPeerName+"* pPeer);";
        }
        protected override String GetOpeningForSource()
        {
            return "#include \"../CommmonCPP/stdafx.h\"\n" +
                "#include \"../CommmonCPP/" + mPeerName + ".h\"\n" +
                "#include \"../CommmonCPP/" + mPeerName + ".cpp\"\n" +
                "#include \"../CommmonCPP/" + mBufferClassName + ".h\"\n" +
                "#include \"../CommmonCPP/" + mBufferClassName + ".cpp\"\n" +
                "#include \"" + mClassName+".h\"\n" +
                "\n\nbool " + mClassName +"::Process(string buffer){" +
                "\n\tunsigned char* decryptedPacket = mpPeer->RecvData(buffer);" +
                "\n\t" + mBufferClassName + " recvBuffer{decryptedPacket};" +
                "\n\tuint32_t nProtocolNum = 0;" +
                "\n\trecvBuffer.GetUInt32(nProtocolNum);" +
                "\n\tbool bProcess = (this->*mFuncList[nProtocolNum])(&recvBuffer);" +
                "\n\tif(!bProcess)" +
                "\n\t{" +
                "\n\t\t return false;" +
                "\n\t}" +
                "\n\n\treturn true;" +
                "\n}";
        }
        protected override String GetEndingForHeader()
        {
            // �ļ� �Լ������ ���� ������ ������ �ݹ��Լ� ���� ���ڿ��� ������
            String strEnum = "\n\tenum PROTOCOLS{";
            foreach (var funcName in mDelegateList)
            {
                strEnum += "\n\t\t" + funcName.ToString().ToUpper() + ", ";
            }
            strEnum += "\n\t\tPROTOCOL_MAX, ";
            // ������ ��ǥ ����
            if (strEnum.Length >= 2)
                strEnum = strEnum.Remove(strEnum.Length - 2);

            return
                strEnum + "};\n\n\t" +
                "typedef bool(" + mClassName + "::" + "*FuncType)(" + mBufferClassName + "*);\n\t" +
                "FuncType mFuncList[PROTOCOLS::PROTOCOL_MAX];\n\t" +
                "bool Process(string buffer);\n};";
        }
        protected override String GetEndingForSource()
        {
            String strGenerator = "";
            foreach (var funcName in mDelegateList)
            {
                strGenerator += "\n\tmFuncList[PROTOCOLS::" + funcName.ToString().ToUpper()+"] =" +
                    " &" + mClassName + "::" + funcName.ToString() + ";";
            }

            return "\n\n" + mClassName + "::" + mClassName + "(" + mPeerName + "* pPeer)" +
                "\n{" +
                "\n\tmpPeer = pPeer;" +
                strGenerator+
                "\n}";
        }
        protected override String GetDefaultParam(PROTOCOL_FUNCTION_TYPE type)
        {
            switch(type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    return mBufferClassName + "* pBuffer";
                default:
                    return "";
            }
        }
        protected override String GetParamForHeader(PROTOCOL_FUNCTION_TYPE type, String paramType, String paramName)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.SEND:
                case PROTOCOL_FUNCTION_TYPE.RECV:
                    return GetVarTypeName(paramType) + " " + paramName + ", ";
                default:
                    return "";
            }
        }
        protected override String GetParamForSource(PROTOCOL_FUNCTION_TYPE type, String paramType, String paramName)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    return GetVarTypeName(paramType) + " " + paramName + ", ";
                default:
                    return "";
            }
        }
        protected override String GetParamBriefForHeader(PROTOCOL_FUNCTION_TYPE type, String paramName, String paramBrief)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.SEND:
                case PROTOCOL_FUNCTION_TYPE.RECV:
                    return "\t\t" + paramName + ": " + paramBrief + "\n";
                default:
                    return "";
            }
        }
        protected override String GetParamBriefForSource(PROTOCOL_FUNCTION_TYPE type, String paramName, String paramBrief)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    return "\t" + paramName + ": " + paramBrief + "\n";
                default:
                    return "";
            }
        }
        protected override String GetFuncNameForHeader(PROTOCOL_FUNCTION_TYPE type, String funcName)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    {
                        mDelegateList.Add("pp" + funcName);
                        return "\tbool pp" + funcName + "(";
                    }
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    return "\tbool sf" + funcName + "(";
                // recv �Լ��� ��ӹ޴� �������� �ڵ鷯���� ������ �� �ֵ��� ���������Լ��� ������
                case PROTOCOL_FUNCTION_TYPE.RECV:
                    return "\tvirtual bool rf" + funcName + "(";
                default:
                    return "";
            }
        }
        protected override String GetFuncNameForSource(PROTOCOL_FUNCTION_TYPE type, String funcName)
        {
            switch(type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    return "bool " + mClassName + "::pp" + funcName + "(";
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    return "bool " + mClassName + "::sf" + funcName + "(";
                default:
                    return "";
            }
        }
        protected override String GetFuncBriefForHeader(PROTOCOL_FUNCTION_TYPE type, String funcBrief)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    return "\n\t/*\tparser func";
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    return "\n\t/*\tsend func\n\t\t" + funcBrief + " \n";
                case PROTOCOL_FUNCTION_TYPE.RECV:
                    return "\n\t/*\trecv func\n\t\t" + funcBrief + " \n";
                default:
                    return "";
            }
        }
        protected override String GetFuncBriefForSource(PROTOCOL_FUNCTION_TYPE type, String funcBrief)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    return "\n/*\tparser func";
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    return "\n/*\tsend func\n\t" + funcBrief + " \n";
                default:
                    return "";
            }
        }
        protected override String GetFuncCommentForHeader(PROTOCOL_FUNCTION_TYPE type, String funcName, Dictionary<String, String> paramList)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    {
                        return ");\n";
                    }
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    {
                        return ");\n";
                    }
                case PROTOCOL_FUNCTION_TYPE.RECV:
                    return ") = 0;\n";
                default:
                    return "";
            }
        }
        protected override String GetFuncCommentForSource(PROTOCOL_FUNCTION_TYPE type, String funcName, Dictionary<String, String> paramList)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    {
                        // buffer �� Ǯ�� �Ű������鿡 ��� recv �Լ��� �ѱ�� ����
                        // ���� �Ľ��� ���� ���� ����
                        String bufferVariables = "";
                        // ���� �Ľ��ϴ� ����
                        String bufferParse = "\n";
                        // recv �Լ��� �ѱ�� ����
                        String recvDesc = "return rf" + funcName + "(";
                        foreach (KeyValuePair<String, String> row in paramList)
                        {
                            String variable = row.Key;
                            bufferVariables += "\t" + GetVarTypeName(row.Value) + " " + variable + ";\n";
                            bufferParse += "\tpBuff->Get" + GetBufferFunc(row.Value.ToString()) + "(" + variable + ");\n";
                            recvDesc += variable + ", ";
                        }
                        // ������ ��ǥ�� �Լ� ��ȣó�� ������
                        recvDesc = recvDesc.Remove(recvDesc.Length - 2) + ");";
                        // recv �Լ��� �ѱ�� ����
                        return ")\n{\n" + bufferVariables + bufferParse + "\n\t" + recvDesc + "\n}\n";
                    }
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    {
                        // ���ۿ� ��Ŷ�� ��� send �Ѵ�.
                        String packetAdd = "\t" + mBufferClassName + " buffer;\n";
                        String protocolNumType = GetBufferFunc(mDelegateList.Count.GetType().ToString());
                        packetAdd += "\tbuffer.Add"+ protocolNumType + "(PROTOCOLS::PP"+ funcName.ToUpper() + ");\n";
                        foreach (KeyValuePair<String, String> row in paramList)
                        {
                            packetAdd += "\tbuffer.Add" + GetBufferFunc(row.Value.ToString()) + "(" + row.Key + ");\n";
                        }
                        return ")\n{\n" + packetAdd + "\n\treturn mpPeer->SendData(&buffer);\n}\n";
                    }
                default:
                    return "";
            }
        }
        protected override String GetFuncCatForHeader(PROTOCOL_FUNCTION_TYPE type, String funcComment, String funcBrief)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                case PROTOCOL_FUNCTION_TYPE.SEND:
                case PROTOCOL_FUNCTION_TYPE.RECV:
                    {
                        return funcBrief + "\t*/\n" + funcComment;
                    }
                default:
                    return "";
            }
        }
        // �������� �Լ� ���� ����
        protected override String GetFuncCatForSource(PROTOCOL_FUNCTION_TYPE type, String funcComment, String funcBrief)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    {
                        return funcBrief + "*/\n" + funcComment;
                    }
                default:
                    return "";
            }
        }
        protected override string GetBufferFunc(string type)
        {
            switch (type)
            {
                case "string":
                    return "String";
                case "float":
                    return "Float";
                case "double":
                    return "Double";
                case "uint8":
                    return "UInt8";
                case "uint16":
                    return "UInt16";
                case "uint32":
                case "System.Int32":
                    return "UInt32";
                case "uint64":
                    return "UInt64";
                default:
                    return "";
            }
        }
        protected override String GetVarTypeName(String type)
        {
            switch (type)
            {
                case "string":
                    return "string";
                case "float":
                    return "float";
                case "double":
                    return "double";
                case "uint8":
                    return "uint8_t";
                case "uint16":
                    return "uint16_t";
                case "uint32":
                    return "uint32_t";
                case "uint64":
                    return "uint64_t";
                default:
                    return "";
            }
        }
        protected override String GetVarDefaultType(String type)
        {
            return "";
        }
    }

}