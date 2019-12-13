using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task5
{
    class Program
	{
		private static String fileName = "SerializedList.json";
		private static String fileNameEmpty = "SerializedListEmpty.json";

        ///
        /// I assumed that Head and Tail are always references to first/last list node,
        /// that Count always represents current amount of list nodes, and when Count == 0 Head and Tail == null
		///
		static void Main(String[] args)
        {
			ListNode testNode1 = new ListNode();
			ListNode testNode2 = new ListNode();
			ListNode testNode3 = new ListNode();
			ListNode testNode4 = new ListNode();

			testNode1.Next = testNode2;
			testNode2.Next = testNode3;
			testNode3.Next = testNode4;
			testNode4.Next = null;

			testNode1.Prev = null;
			testNode2.Prev = testNode1;
			testNode3.Prev = testNode2;
			testNode4.Prev = testNode3;

			testNode1.Data = "TestNode1";
			testNode2.Data = "TestNode2";
			testNode3.Data = "TestNode3";
			testNode4.Data = "TestNode4";

			testNode1.Rand = testNode3;
			testNode2.Rand = testNode4;
			testNode3.Rand = testNode1;
			testNode4.Rand = testNode1;

			ListRand list = new ListRand
			{
				Head = testNode1,
				Tail = testNode4,
				Count = 4
			};

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
				list.Serialize(fs);

            using (var fs = new FileStream(fileNameEmpty, FileMode.Create, FileAccess.ReadWrite))
                new ListRand().Serialize(fs);

			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
			{
				ListRand deserializedList = new ListRand();
				deserializedList.Deserialize(fs);
			}

			using (FileStream fs = new FileStream(fileNameEmpty, FileMode.Open, FileAccess.ReadWrite))
			{
				ListRand deserializedList = new ListRand();
				deserializedList.Deserialize(fs);
			}
        }
    }
}
