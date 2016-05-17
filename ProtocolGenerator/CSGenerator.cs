using System;
using System.Data;
using System.Collections.Generic;
using Common;

namespace ProtocolGenerator
{
    public class CSGenerator : ProtocolGenerator
    {
        public CSGenerator(DataSet dataSet): base(dataSet)
        {

        }
        protected override String GetOpeningForHeader()
        {
            return "";
        }
        protected override String GetOpeningForSource()
        {
            return
                "using Common;\nusing System;\n\n" +
                "abstract class " + mClassName +
                "\n{" +
                "\n\tprotected " + mPeerName + " mPeer;" +
                "\n\t\t";
        }
        protected override String GetEndingForHeader()
        {
            return "";
        }
        protected override String GetEndingForSource()
        {
            // 파서 함수목록을 통해 열거형 변수와 콜백함수 생성 문자열을 만들자
            String strEnum = "\n\tenum PROTOCOLS{";
            String strGenerator = "";
            foreach(var funcName in mDelegateList)
            {
                strEnum += "\n\t\t" + funcName.ToString().ToUpper() + ", ";
                strGenerator += "\n\t\t\tnew mStrDelegate("+ funcName.ToString() + "), ";
            }
            // 마지막 쉼표 제거
            if (strEnum.Length >= 2)
                strEnum = strEnum.Remove(strEnum.Length - 2);
            // 마지막 쉼표 제거
            if (strGenerator.Length >= 2)
                strGenerator = strGenerator.Remove(strGenerator.Length - 2);
            strEnum += "\n\t}";
            
            return
                strEnum + 
                "\n\n\tprivate delegate bool mStrDelegate(" + mBufferClassName + " buffer);" +
                "\n\tprivate mStrDelegate[] mDelegateList;" +
                "\n\n\tprotected " + mClassName + "(" + mPeerName + " peer)" +
                "\n\t{" +
                "\n\t\tmPeer = peer;" +
                "\n\t\tmDelegateList = new mStrDelegate[]{" +
                strGenerator +
                "\n\t\t};" +
                "\n\t}" +
                "\n\n\tpublic bool Process(string buffer){" +
                "\n\t\tbyte[] decryptedPacket = mPeer.RecvData(buffer);" +
                "\n\t\t" + mBufferClassName + " recvBuffer = new " + mBufferClassName + "(decryptedPacket);" +
                "\n\t\tUInt32 nProtocolNum = 0;" +
                "\n\t\trecvBuffer.GetUInt32(ref nProtocolNum);" +
                "\n\t\tbool bProcess = mDelegateList[nProtocolNum](recvBuffer);" +
                "\n\t\tif(!bProcess)" +
                "\n\t\t{" +
                "\n\t\t\t return false;" +
                "\n\t\t}" +
                "\n\t\treturn true;" +
                "\n\t}" + 
                "\n}";
        }
        protected override String GetDefaultParam(PROTOCOL_FUNCTION_TYPE type)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    return mBufferClassName + " buffer";
                default:
                    return "";
            }
        }
        protected override String GetParamForHeader(PROTOCOL_FUNCTION_TYPE type, String paramType, String paramName)
        {
            return "";
        }
        protected override String GetParamForSource(PROTOCOL_FUNCTION_TYPE type, String paramType, String paramName)
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
        protected override String GetParamBriefForHeader(PROTOCOL_FUNCTION_TYPE type, String paramName, String paramBrief)
        {
            return "";
        }
        protected override String GetParamBriefForSource(PROTOCOL_FUNCTION_TYPE type, String paramName, String paramBrief)
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
        protected override String GetFuncNameForHeader(PROTOCOL_FUNCTION_TYPE type, String funcName)
        {
            return "";
        }
        protected override String GetFuncNameForSource(PROTOCOL_FUNCTION_TYPE type, String funcName)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    {
                        mDelegateList.Add("pp" + funcName);
                        return "\tpublic bool pp" + funcName + "(";
                    }
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    return "\tpublic bool sf" + funcName + "(";
                // recv 함수는 recv 후 구현 내용이 들어가야 하므로 순수가상함수로 반드시 재구현 하도록 하자.
                case PROTOCOL_FUNCTION_TYPE.RECV:
                    return "\tpublic abstract bool rf" + funcName + "(";
                default:
                    return "";
            }
        }
        protected override String GetFuncBriefForHeader(PROTOCOL_FUNCTION_TYPE type, String funcBrief)
        {
            return "";
        }
        protected override String GetFuncBriefForSource(PROTOCOL_FUNCTION_TYPE type, String funcBrief)
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
        protected override String GetFuncCommentForHeader(PROTOCOL_FUNCTION_TYPE type, String funcName, Dictionary<String, String> paramList)
        {
            return "";
        }
        protected override String GetFuncCommentForSource(PROTOCOL_FUNCTION_TYPE type, String funcName, Dictionary<String, String> paramList)
        {
            switch (type)
            {
                case PROTOCOL_FUNCTION_TYPE.PARSER:
                    {
                        // buffer 를 풀고 매개변수들에 담아 recv 함수로 넘기는 과정
                        // 버퍼 파싱을 위한 변수 내용
                        String bufferVariables = "";
                        // 버퍼 파싱하는 내용
                        String bufferParse = "\n";
                        // recv 함수로 넘기는 내용
                        String recvDesc = "return rf" + funcName + "(";
                        foreach (KeyValuePair<String, String> row in paramList)
                        {
                            String variable = row.Key;
                            bufferVariables += "\t\t" + GetVarTypeName(row.Value.ToString()) + " " + variable + " = " + GetVarDefaultType(row.Value.ToString()) + ";\n";
                            bufferParse += "\t\tbuff.Get" + GetBufferFunc(row.Value.ToString()) + "(ref " + variable + ");\n";
                            recvDesc += variable + ", ";
                        }
                        // 마지막 쉼표랑 함수 괄호처리 해주지
                        recvDesc = recvDesc.Remove(recvDesc.Length - 2) + ");";
                        // recv 함수로 넘기는 과정
                        return ")\n\t{\n" + bufferVariables + bufferParse + "\n\t\t" + recvDesc + "\n\t}\n";
                    }
                case PROTOCOL_FUNCTION_TYPE.SEND:
                    {
                        // 버퍼에 패킷을 담아 send 한다.
                        String packetAdd = "\t\t" + mBufferClassName + " buffer = new " + mBufferClassName + "();\n";
                        String protocolNumType = GetBufferFunc(mDelegateList.Count.GetType().ToString());
                        packetAdd += "\t\tbuffer.Add"+ protocolNumType + "(Convert.To"+ protocolNumType + "(PROTOCOLS.PP"+ funcName.ToUpper() + "));\n";
                        foreach (KeyValuePair<String, String> row in paramList)
                        {
                            packetAdd += "\t\tbuffer.Add" + GetBufferFunc(row.Value.ToString()) + "(" + row.Key + ");\n";
                        }

                        return ")\n\t{\n" + packetAdd + "\n\t\treturn mPeer.SendData(buffer);\n\t}\n";
                    }
                case PROTOCOL_FUNCTION_TYPE.RECV:
                    return ");\n";
                default:
                    return "";
            }
        }
        protected override String GetFuncCatForHeader(PROTOCOL_FUNCTION_TYPE type, String funcComment, String funcBrief)
        {
            return "";
        }
        // 프로토콜 함수 본문 조합
        protected override String GetFuncCatForSource(PROTOCOL_FUNCTION_TYPE type, String funcComment, String funcBrief)
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
        protected override String GetBufferFunc(String type)
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
                    return "byte";
                case "uint16":
                    return "UInt16";
                case "uint32":
                    return "UInt32";
                case "uint64":
                    return "UInt64";
                default:
                    return "";
            }
        }
        protected override String GetVarDefaultType(String type)
        {
            switch (type)
            {
                case "string":
                    return "\"\"";
                case "float":
                    return "0.0f";
                case "double":
                    return "0.0";
                case "uint8":
                    return "0";
                case "uint16":
                    return "0";
                case "uint32":
                    return "0";
                case "uint64":
                    return "0";
                default:
                    return "";
            }
        }
    }
}