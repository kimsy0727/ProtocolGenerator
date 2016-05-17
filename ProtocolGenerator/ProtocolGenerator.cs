using System;
using System.Data;
using System.Collections.Generic;
using Common;
using System.Collections;

namespace ProtocolGenerator
{
    public abstract class ProtocolGenerator
    {
        // �������� �Լ� ���� 
        public enum PROTOCOL_FUNCTION_TYPE
        {
            PARSER, // ��Ŷ �ļ� �������� �Լ�
            SEND,   // ������ �������� �Լ�
            RECV,   // �ޱ� �������� �Լ�
            MAX
        };

        protected String mBufferClassName = "NetBuffer";
        protected String mClassName = "ProtocolHandler";
        protected String mPeerName = "NetPeer";
        protected ArrayList mDelegateList;
        protected DataSet mDataSet;

        public FileManager mFileManager;
        public ProtocolGenerator(DataSet dataSet)
        {
            mDataSet = dataSet;
            mDelegateList = new ArrayList();
        }

        // ��� ���Ͽ� ���̴� ����
        protected abstract String GetOpeningForHeader();
        // �ҽ� ���Ͽ� ���̴� ����
        protected abstract String GetOpeningForSource();
        // ��� ���Ͽ� ���̴� ������
        protected abstract String GetEndingForHeader();
        // �ҽ� ���Ͽ� ���̴� ������
        protected abstract String GetEndingForSource();
        // �������� �Լ��� ����Ʈ �Ű����� ��������
        protected abstract String GetDefaultParam(PROTOCOL_FUNCTION_TYPE type);
        // ��� ���Ͽ� ���̴� �������� �Լ��� �Ű����� ��������
        protected abstract String GetParamForHeader(PROTOCOL_FUNCTION_TYPE type, String paramType, String paramName);
        // �ҽ� ���Ͽ� ���̴� �������� �Լ��� �Ű����� ��������
        protected abstract String GetParamForSource(PROTOCOL_FUNCTION_TYPE type, String paramType, String paramName);
        // ��� ���Ͽ� ���̴� �������� �Լ��� �Ű����� �ּ� ��������
        protected abstract String GetParamBriefForHeader(PROTOCOL_FUNCTION_TYPE type, String paramName, String paramBrief);
        // �ҽ� ���Ͽ� ���̴� �������� �Լ��� �Ű����� �ּ� ��������
        protected abstract String GetParamBriefForSource(PROTOCOL_FUNCTION_TYPE type, String paramName, String paramBrief);
        // ��� ���Ͽ� ���̴� �������� �Լ� �ּ� ��������
        protected abstract String GetFuncBriefForHeader(PROTOCOL_FUNCTION_TYPE type, String funcBrief);
        // �ҽ� ���Ͽ� ���̴� �������� �Լ� �ּ� ��������
        protected abstract String GetFuncBriefForSource(PROTOCOL_FUNCTION_TYPE type, String funcBrief);
        // ��� ���Ͽ� ���̴� �������� �Լ� �̸� ��������
        protected abstract String GetFuncNameForHeader(PROTOCOL_FUNCTION_TYPE type, String funcName);
        // �ҽ� ���Ͽ� ���̴� �������� �Լ� �̸� ��������
        protected abstract String GetFuncNameForSource(PROTOCOL_FUNCTION_TYPE type, String funcName);
        // ��� ���Ͽ� ���̴� �������� �Լ� ��� ��������
        protected abstract String GetFuncCommentForHeader(PROTOCOL_FUNCTION_TYPE type, String funcName, Dictionary<String, String> paramList);
        // �ҽ� ���Ͽ� ���̴� �������� �Լ� ���� ��������
        protected abstract String GetFuncCommentForSource(PROTOCOL_FUNCTION_TYPE type, String funcName, Dictionary<String, String> paramList);
        // ��� ���Ͽ� ���̴� �������� �Լ� ���� ����
        protected abstract String GetFuncCatForHeader(PROTOCOL_FUNCTION_TYPE type, String funcComment, String funcBrief);
        // �ҽ� ���Ͽ� ���̴� �������� �Լ� ���� ����
        protected abstract String GetFuncCatForSource(PROTOCOL_FUNCTION_TYPE type, String funcComment, String funcBrief);
        // ���ۿ� ���� �Լ� ��������
        protected abstract String GetBufferFunc(String type);
        // ���� Ÿ���̸��� ���� �Լ� ��������
        protected abstract String GetVarTypeName(String type);
        // ���� Ÿ�ӿ� ���� ����Ʈ���� ��������
        protected abstract String GetVarDefaultType(String type);
        // �ڵ� ���� ����
        public Boolean Generate(ref String headerComment, ref String sourceComment)
        {
            // ������ ���̹��� ���̺��� ��������.
            string tableName = "PROTOCOLS";
            DataTable table = mDataSet.Tables[tableName];

            headerComment += GetOpeningForHeader(); 
            sourceComment += GetOpeningForSource();

            for(PROTOCOL_FUNCTION_TYPE i=0; i<PROTOCOL_FUNCTION_TYPE.MAX; ++i)
            {
                // ���� row �� ã�� ���� ���� relation ������ ���� (��,�ҹ��� ���о��� ��� �����ϱ� ����)
                foreach (DataRelation relation in table.ChildRelations)
                {
                    // relation �� �ش��ϴ� row ���� ��������
                    foreach (DataRow row in table.Rows[0].GetChildRows(relation.ToString()))
                    {
                        // ��� column �� value �� �������� ����
                        // row �� ��� column ���� ��������
                        String funcBrief = "";
                        String funcNameForHeader = "";
                        String funcNameForSource = "";
                        foreach(DataColumn column in row.Table.Columns)
                        {
                            // column �߿��� �ڵ����� ������ hidden column ���� ��������
                            if(column.ColumnMapping != MappingType.Hidden)
                            {
                                /*
                                 * �Լ� ����
                                 */
                                if(column.ToString() == "brief")
                                {
                                    funcBrief = row[column.ToString()].ToString();
                                }
                                else if(column.ToString() == "name")
                                {
                                    funcNameForHeader = GetFuncNameForHeader(i, row[column.ToString()].ToString());
                                    funcNameForSource = GetFuncNameForSource(i, row[column.ToString()].ToString());
                                }
                                //Console.Write("\t" + row[column.ToString()]);
                            }
                        }
                        headerComment += GetFuncBriefForHeader(i, funcBrief);
                        sourceComment += GetFuncBriefForSource(i, funcBrief);

                        // �Լ� ���� �ۼ��� ���� �Ű����� �����̳�
                        Dictionary<String, String> paramList = new Dictionary<string, string>();
                        String funcHeaderParamBrief = "";
                        String funcSourceParamBrief = "";
                        String funcHeaderComment = funcNameForHeader + GetDefaultParam(i);
                        String funcSourceComment = funcNameForSource + GetDefaultParam(i);
                        // �˻��� row �� childrow �� �������� ���� ��� relation �� �������� (��,�ҹ��� ���о��� ��� �����ϱ� ����)
                        foreach(DataRelation childRelation in row.Table.ChildRelations)
                        {
                            // relation �� �ش��ϴ� child row �� ��������.
                            foreach (DataRow childRow in row.GetChildRows(childRelation.ToString()))
                            {
                                // ��� column �� value �� �������� ����
                                // child row �� ��� column ���� ��������
                                String paramBrief = "";
                                String paramName = "";
                                String paramType = "";
                                foreach (DataColumn childColumn in childRow.Table.Columns)
                                {
                                    // column �߿��� �ڵ����� ������ hidden column ���� ��������
                                    if (childColumn.ColumnMapping != MappingType.Hidden)
                                    {
                                        /*
                                         * �Ű����� ����
                                         */
                                        if (childColumn.ToString() == "brief")
                                        {
                                            paramBrief = childRow[childColumn.ToString()].ToString();
                                        }
                                        else if (childColumn.ToString() == "name")
                                        {
                                            paramName = childRow[childColumn.ToString()].ToString();
                                        }
                                        else if (childColumn.ToString() == "type")
                                        {
                                            paramType = childRow[childColumn.ToString()].ToString();
                                        }
                                        //Console.Write("\t\t" + childRow[childColumn.ToString()]);
                                    }
                                }

                                funcHeaderComment += GetParamForHeader(i, paramType, paramName);
                                funcSourceComment += GetParamForSource(i, paramType, paramName);
                                funcHeaderParamBrief += GetParamBriefForHeader(i, paramName, paramBrief);
                                funcSourceParamBrief += GetParamBriefForSource(i, paramName, paramBrief);

                                paramList.Add(paramName, paramType);

                                //Console.WriteLine();
                            }
                        }
                        // ������ ��ǥ ����
                        if (funcHeaderComment.Length >= 2)
                            funcHeaderComment = funcHeaderComment.Remove(funcHeaderComment.Length - 2);
                        if (funcSourceComment.Length >= 2)
                            funcSourceComment = funcSourceComment.Remove(funcSourceComment.Length - 2);
                        // �Լ� ���� ��������
                        funcHeaderComment += GetFuncCommentForHeader(i, row["name"].ToString(), paramList);
                        funcSourceComment += GetFuncCommentForSource(i, row["name"].ToString(), paramList);

                        headerComment += GetFuncCatForHeader(i, funcHeaderComment, funcHeaderParamBrief);
                        sourceComment += GetFuncCatForSource(i, funcSourceComment, funcSourceParamBrief);
                    }
                }            
            }

            headerComment += GetEndingForHeader();
            sourceComment += GetEndingForSource();

            return true;
        }
    }
}