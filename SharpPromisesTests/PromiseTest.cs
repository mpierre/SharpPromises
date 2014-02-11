using NUnit.Framework;
using System;
using SharpPromises;

namespace SharpPromisesTests
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void TestSimpleDone ()
		{
			var deferred = new Deferred<int>();
			int result = 0;
			deferred.Done(v => result = v);
			Assert.AreEqual(result, 0);
			deferred.Resolve(10);
			Assert.AreEqual(result, 10);
		}

		[Test ()]
		public void TestAfterTheFactDone ()
		{
			var deferred = new Deferred<int>();
			int result = 0;
			deferred.Resolve(10);
			deferred.Done(v => result = v);
			Assert.AreEqual(result, 10);
		}

		[Test ()]
		public void TestChainedDone ()
		{
			var deferred = new Deferred<int>();
			int result = 0;
			deferred.Done(v => result = v * 2);
			deferred.Done(v => result /= v);
			Assert.AreEqual(0, result);
			deferred.Resolve(10);
			Assert.AreEqual(2, result);
		}

		[Test ()]
		public void TestDoneIdempotence ()
		{
			var deferred = new Deferred<int>();
			int result = 0;
			deferred.Done(v => result += v);
			deferred.Fail(e => result += 300);
			deferred.Resolve(10);
			deferred.Resolve(20);
			deferred.Reject(new Exception());
			Assert.AreEqual(10, result);
		}


		[Test ()]
		public void TestSimpleFail()
		{
			var deferred = new Deferred<int>();
			Exception result = null;
			deferred.Fail(e => result = e);
			Assert.AreEqual(result, null);
			var expected = new Exception();
			deferred.Reject(expected);
			Assert.AreEqual(result, expected);
		}

		[Test ()]
		public void TestAfterTheFactFail()
		{
			var deferred = new Deferred<int>();
			Exception result = null;
			var expected = new Exception();
			deferred.Reject(expected);
			deferred.Fail(e => result = e);
			Assert.AreEqual(result, expected);
		}

		[Test ()]
		public void TestFailIdempotence()
		{
			var deferred = new Deferred<string>();
			string result = "";
			deferred.Done(r => result += r);
			deferred.Fail(e => result += e.Message);
			deferred.Reject(new Exception("Hello"));
			deferred.Reject(new Exception("Banana"));
			deferred.Resolve("banana");
			Assert.AreEqual(result, "Hello");
		}

		[Test ()]
		public void TestThen()
		{
			var deferred = new Deferred<int>();
			string result = null;
			deferred
				.Then<string>(v => "Hello "+v)
				.Done(r => result = r);
			deferred.Resolve(23);
			Assert.AreEqual("Hello 23", result);
		}

		private Promise<string> CreatePromise<T>(T value)
		{
			return new Deferred<string>().Resolve(value.ToString());
		}

		[Test ()]
		public void TestThenWithPromise()
		{
			var deferred = new Deferred<int>();
			string result = null;
			deferred
				.Done(r => result = "Hello " + r)
				.Then<string> (v => { return CreatePromise (v); })
				.Done(r => result = result + " " + r.Length);
			deferred.Resolve(23);
			Assert.AreEqual("Hello 23 2", result);
		}
	}
}

