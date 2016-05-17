using System;
using System.Data;
using System.Collections.Generic;
using Common;
using System.Collections;

namespace ProtocolGenerator
{
    public abstract class ProtocolGenerator
    {
        // 프로토콜 함수 종류 
        public enum PROTOCOL_FUNCTION_TYPE
        {
            PARSER, // 패킷 파서 프로토콜 함수
            SEND,   // 보내기 프로토콜 함수
            RECV,   // 받기 프로토콜 함수
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

        // 헤더 파일에 쓰이는 서두
        protected abstract String GetOpeningForHeader();
        // 소스 파일에 쓰이는 서두
        protected abstract String GetOpeningForSource();
        // 헤더 파일에 쓰이는 맺음말
        protected abstract String GetEndingForHeader();
        // 소스 파일에 쓰이는 맺음말
        protected abstract String GetEndingForSource();
        // 프로토콜 함수의 디폴트 매개변수 가져오기
        protected abstract String GetDefaultParam(PROTOCOL_FUNCTION_TYPE type);
        // 헤더 파일에 쓰이는 프로토콜 함수의 매개변수 가져오기
        protected abstract String GetParamForHeader(PROTOCOL_FUNCTION_TYPE type, String paramType, String paramName);
        // 소스 파일에 쓰이는 프로토콜 함수의 매개변수 가져오기
        protected abstract String GetParamForSource(PROTOCOL_FUNCTION_TYPE type, String paramType, String paramName);
        // 헤더 파일에 쓰이는 프로토콜 함수의 매개변수 주석 가져오기
        protected abstract String GetParamBriefForHeader(PROTOCOL_FUNCTION_TYPE type, String paramName, String paramBrief);
        // 소스 파일에 쓰이는 프로토콜 함수의 매개변수 주석 가져오기
        protected abstract String GetParamBriefForSource(PROTOCOL_FUNCTION_TYPE type, String paramName, String paramBrief);
        // 헤더 파일에 쓰이는 프로토콜 함수 주석 가져오기
        protected abstract String GetFuncBriefForHeader(PROTOCOL_FUNCTION_TYPE type, String funcBrief);
        // 소스 파일에 쓰이는 프로토콜 함수 주석 가져오기
        protected abstract String GetFuncBriefForSource(PROTOCOL_FUNCTION_TYPE type, String funcBrief);
        // 헤더 파일에 쓰이는 프로토콜 함수 이름 가져오기
        protected abstract String GetFuncNameForHeader(PROTOCOL_FUNCTION_TYPE type, String funcName);
        // 소스 파일에 쓰이는 프로토콜 함수 이름 가져오기
        protected abstract String GetFuncNameForSource(PROTOCOL_FUNCTION_TYPE type, String funcName);
        // 헤더 파일에 쓰이는 프로토콜 함수 헤더 가져오기
        protected abstract String GetFuncCommentForHeader(PROTOCOL_FUNCTION_TYPE type, String funcName, Dictionary<String, String> paramList);
        // 소스 파일에 쓰이는 프로토콜 함수 본문 가져오기
        protected abstract String GetFuncCommentForSource(PROTOCOL_FUNCTION_TYPE type, String funcName, Dictionary<String, String> paramList);
        // 헤더 파일에 쓰이는 프로토콜 함수 본문 조합
        protected abstract String GetFuncCatForHeader(PROTOCOL_FUNCTION_TYPE type, String funcComment, String funcBrief);
        // 소스 파일에 쓰이는 프로토콜 함수 본문 조합
        protected abstract String GetFuncCatForSource(PROTOCOL_FUNCTION_TYPE type, String funcComment, String funcBrief);
        // 버퍼에 대한 함수 가져오기
        protected abstract String GetBufferFunc(String type);
        // 변수 타입이름에 대한 함수 가져오기
        protected abstract String GetVarTypeName(String type);
        // 변수 타임에 대한 디폴트변수 가져오기
        protected abstract String GetVarDefaultType(String type);
        // 코드 추출 로직
        public Boolean Generate(ref String headerComment, ref String sourceComment)
        {
            // 선택한 네이밍의 테이블을 가져오자.
            string tableName = "PROTOCOLS";
            DataTable table = mDataSet.Tables[tableName];

            headerComment += GetOpeningForHeader(); 
            sourceComment += GetOpeningForSource();

            for(PROTOCOL_FUNCTION_TYPE i=0; i<PROTOCOL_FUNCTION_TYPE.MAX; ++i)
            {
                // 하위 row 를 찾기 위해 하위 relation 루프를 돌자 (대,소문자 구분없이 모두 포함하기 위해)
                foreach (DataRelation relation in table.ChildRelations)
                {
                    // relation 에 해당하는 row 들을 루프돌자
                    foreach (DataRow row in table.Rows[0].GetChildRows(relation.ToString()))
                    {
                        // 모든 column 과 value 를 가져오기 위해
                        // row 의 모든 column 들을 루프돌자
                        String funcBrief = "";
                        String funcNameForHeader = "";
                        String funcNameForSource = "";
                        foreach(DataColumn column in row.Table.Columns)
                        {
                            // column 중에는 자동으로 생성된 hidden column 들은 제외하자
                            if(column.ColumnMapping != MappingType.Hidden)
                            {
                                /*
                                 * 함수 정의
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

                        // 함수 본문 작성을 위한 매개변수 컨테이너
                        Dictionary<String, String> paramList = new Dictionary<string, string>();
                        String funcHeaderParamBrief = "";
                        String funcSourceParamBrief = "";
                        String funcHeaderComment = funcNameForHeader + GetDefaultParam(i);
                        String funcSourceComment = funcNameForSource + GetDefaultParam(i);
                        // 검색한 row 의 childrow 를 루프돌기 위해 모든 relation 을 가져오자 (대,소문자 구분없이 모두 포함하기 위해)
                        foreach(DataRelation childRelation in row.Table.ChildRelations)
                        {
                            // relation 에 해당하는 child row 를 루프돌자.
                            foreach (DataRow childRow in row.GetChildRows(childRelation.ToString()))
                            {
                                // 모든 column 과 value 를 가져오기 위해
                                // child row 의 모든 column 들을 루프돌자
                                String paramBrief = "";
                                String paramName = "";
                                String paramType = "";
                                foreach (DataColumn childColumn in childRow.Table.Columns)
                                {
                                    // column 중에는 자동으로 생성된 hidden column 들은 제외하자
                                    if (childColumn.ColumnMapping != MappingType.Hidden)
                                    {
                                        /*
                                         * 매개변수 정의
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
                        // 마지막 쉼표 제거
                        if (funcHeaderComment.Length >= 2)
                            funcHeaderComment = funcHeaderComment.Remove(funcHeaderComment.Length - 2);
                        if (funcSourceComment.Length >= 2)
                            funcSourceComment = funcSourceComment.Remove(funcSourceComment.Length - 2);
                        // 함수 본문 가져오기
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