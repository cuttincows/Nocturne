using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class InterpreterTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestBlockSplit ()
        {
            GameObject go = new GameObject();
            LanguageInterpreter interpreter = go.AddComponent<LanguageInterpreter>();

            Dictionary<string, List<string>> res = interpreter.SplitBlocks(new string[] {
                "block test:",
                "\tsay \"Hello, test!\"",
                "endblock"
            });

            Assert.IsTrue(res.ContainsKey("test"));
            Assert.True(res["test"][0] == "\tsay \"Hello, test!\"");
        }
    }
}
