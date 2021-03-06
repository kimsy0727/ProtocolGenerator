using Common;
using System;

abstract class ProtocolHandler
{
	protected NetPeer mPeer;
		
	/*	parser func	*/
	public String ppAckResult(NetBuffer buff)
	{
		byte result = 0;
		string msg = "";

		buff.GetUInt8(ref result);
		buff.GetString(ref msg);

		return rfAckResult(result, msg);
	}

	/*	parser func	*/
	public String ppReqTest(NetBuffer buff)
	{
		string test0 = "";
		float test1 = 0.0f;
		double test2 = 0.0;
		byte test3 = 0;
		UInt16 test4 = 0;
		UInt32 test5 = 0;
		UInt64 test6 = 0;

		buff.GetString(ref test0);
		buff.GetFloat(ref test1);
		buff.GetDouble(ref test2);
		buff.GetUInt8(ref test3);
		buff.GetUInt16(ref test4);
		buff.GetUInt32(ref test5);
		buff.GetUInt64(ref test6);

		return rfReqTest(test0, test1, test2, test3, test4, test5, test6);
	}

	/*	parser func	*/
	public String ppAckTest(NetBuffer buff)
	{
		byte result = 0;
		string msg = "";

		buff.GetUInt8(ref result);
		buff.GetString(ref msg);

		return rfAckTest(result, msg);
	}

	/*	send func
		디폴트 응답 프로토콜 
		result: 성공여부, 0이면 성공
		msg: 에러 메시지
	*/
	public String sfAckResult(byte result, string msg)
	{
		NetBuffer buffer = new NetBuffer();
		buffer.AddUInt32(Convert.ToUInt32(PROTOCOLS.PPACKRESULT));
		buffer.AddUInt8(result);
		buffer.AddString(msg);

		return mPeer.AckWebData(buffer);
	}

	/*	send func
		테스트 프로토콜 타입 
		test0: 문자열
		test1: 소수점 4바이트
		test2: 소수점 8바이트
		test3: 정수 1바이트
		test4: 정수 2바이트
		test5: 정수 4바이트
		test6: 정수 8바이트
	*/
	public bool sfReqTest(string test0, float test1, double test2, byte test3, UInt16 test4, UInt32 test5, UInt64 test6)
	{
		NetBuffer buffer = new NetBuffer();
		buffer.AddUInt32(Convert.ToUInt32(PROTOCOLS.PPREQTEST));
		buffer.AddString(test0);
		buffer.AddFloat(test1);
		buffer.AddDouble(test2);
		buffer.AddUInt8(test3);
		buffer.AddUInt16(test4);
		buffer.AddUInt32(test5);
		buffer.AddUInt64(test6);

		return mPeer.SendWebData(buffer);
	}

	/*	send func
		테스트 프로토콜에 대한 응답 
		result: 성공여부, 0이면 성공
		msg: 에러 메시지
	*/
	public String sfAckTest(byte result, string msg)
	{
		NetBuffer buffer = new NetBuffer();
		buffer.AddUInt32(Convert.ToUInt32(PROTOCOLS.PPACKTEST));
		buffer.AddUInt8(result);
		buffer.AddString(msg);

		return mPeer.AckWebData(buffer);
	}

	/*	recv func
		디폴트 응답 프로토콜 
		result: 성공여부, 0이면 성공
		msg: 에러 메시지
	*/
	public abstract String rfAckResult(byte result, string msg);

	/*	recv func
		테스트 프로토콜 타입 
		test0: 문자열
		test1: 소수점 4바이트
		test2: 소수점 8바이트
		test3: 정수 1바이트
		test4: 정수 2바이트
		test5: 정수 4바이트
		test6: 정수 8바이트
	*/
	public abstract String rfReqTest(string test0, float test1, double test2, byte test3, UInt16 test4, UInt32 test5, UInt64 test6);

	/*	recv func
		테스트 프로토콜에 대한 응답 
		result: 성공여부, 0이면 성공
		msg: 에러 메시지
	*/
	public abstract String rfAckTest(byte result, string msg);

	enum PROTOCOLS{
		PPACKRESULT, 
		PPREQTEST, 
		PPACKTEST
	}

	private delegate String mStrDelegate(NetBuffer buffer);
	private mStrDelegate[] mDelegateList;

	protected ProtocolHandler(NetPeer peer)
	{
		mPeer = peer;
		mDelegateList = new mStrDelegate[]{
			new mStrDelegate(ppAckResult), 
			new mStrDelegate(ppReqTest), 
			new mStrDelegate(ppAckTest)
		};
	}

	public String Process(string buffer){
		byte[] decryptedPacket = mPeer.RecvData(buffer);
		NetBuffer recvBuffer = new NetBuffer(decryptedPacket);
		UInt32 nProtocolNum = 0;
		recvBuffer.GetUInt32(ref nProtocolNum);
		String strProcess = mDelegateList[nProtocolNum](recvBuffer);
		return strProcess;
	}
}