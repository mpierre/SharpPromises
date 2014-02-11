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
			deferred.Done(v => { result = v; });
			Assert.AreEqual(result, 0);
			deferred.Resolve(10);
			Assert.AreEqual(result, 10);
		}

		[Test ()]
		public void TestSimpleFail()
		{
			var deferred = new Deferred<int>();
			Exception result = null;
			deferred.Fail(e => { result = e; });
			Assert.AreEqual(result, null);
			var expected = new Exception();
			deferred.Reject(expected);
			Assert.AreEqual(result, expected);
		}
	}
}

