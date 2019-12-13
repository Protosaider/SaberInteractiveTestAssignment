using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Task5 {
	/// <summary>
	/// Json Scheme:
	/// {
	///		"Count": "3",
	///		"Head": "1",
	///		"Tail": "2",
	///or	"Nodes": "null"
	///else	"Nodes": {
	///			"Node": [
	///				{
	///					"Index": "1",
	///					"Data": "abc"
	///					"Prev": "2",
	///					"Rand": "1",
	///					"Next": "3"
	///				},
	///			]
	///		}
	/// }
	/// </summary>
	public static class RandListSerializerHelper
	{
		static readonly String lCurly = "{";
		static readonly String rCurly = "}";
		static readonly String rCurlyComma = "},";
		static readonly String rSquare = "]";
		static readonly String lSquare = "[";
		static readonly String colon = ":";
		static readonly String comma = ",";
		static readonly String quoteComma = "\",";
		static readonly String quote = "\"";
		static readonly String space = " ";
		static readonly String tab = "\t";
		static readonly String tab3 = "\t\t\t";

		static readonly String listCount = "\t\"Count\": \"";
		static readonly String listHead = "\t\"Head\": \"";
		static readonly String listTail = "\t\"Tail\": \"";
		static readonly String listNodes = "\t\"Nodes\": ";

		static readonly String nodeHeader = "\t\t\"Node\": [";
		static readonly String nodeStart = "\t\t\t{";
		static readonly String nodeIndex = "\t\t\t\t\"Index\": \"";
		static readonly String nodePrev = "\t\t\t\t\"Prev\": \"";
		static readonly String nodeNext = "\t\t\t\t\"Next\": \"";
		static readonly String nodeRand = "\t\t\t\t\"Rand\": \"";
		static readonly String nodeData = "\t\t\t\t\"Data\": \"";



		static readonly String signs = @"[\s,{}:""]*";
		static readonly String signsNoCurly = @"[\s,:""]*";

		private static readonly Regex regexCountHeadTail =
			new Regex(
				$@"{signs}Count{signs}(?<Count>\d+)" + 
				$@"{signs}Head{signs}(?<Head>\d+|null)" +
				$@"{signs}Tail{signs}(?<Tail>\d+|null){signs}"
			);

		static readonly Regex regexNodes =
			new Regex($@"\[{signsNoCurly}({signsNoCurly}(?<Node>{{[\w\s,:""]*}}){signsNoCurly})*{signsNoCurly}\]");

		private static readonly Regex regexNode =
			new Regex(
				$@"{signs}Index{signs}(?<Index>\d+)" + 
				$@"{signs}Data"": ""(?<Data>.+)""," + 
				$@"{signs}Prev{signs}(?<Prev>\d+|null)" + 
				$@"{signs}Rand{signs}(?<Rand>\d+|null)" + 
				$@"{signs}Next{signs}(?<Next>\d+|null){signs}"
			);

		static readonly String nullRef = "null";

		public static void DeserializeToString(this ListRand list, String serializedList)
		{
			Match match = regexCountHeadTail.Match(serializedList);

			String countValue = match.Groups["Count"].Value;
			var count = Int32.Parse(countValue);

			//TODO: Assume that count always represents current amount of list nodes
			if (count == 0)
				return;

			list.Count = count;

			Dictionary<Int32, ListNode> indexedNodes = new Dictionary<Int32, ListNode>(list.Count);

			String headValue = match.Groups["Head"].Value;
			String tailValue = match.Groups["Tail"].Value;

			var head = Int32.Parse(headValue);
			var tail = Int32.Parse(tailValue);

			match = regexNodes.Match(serializedList);
			for (var i = 0; i < match.Groups["Node"].Captures.Count; i++)
			{
				ListNode node;
				String nodeValue = match.Groups["Node"].Captures[i].Value;
				var nodeMatch = regexNode.Match(nodeValue);

				String indexValue = nodeMatch.Groups["Index"].Value;
				var index = Int32.Parse(indexValue);

				if (!indexedNodes.ContainsKey(index))
				{
					node = new ListNode();
					indexedNodes.Add(index, node);
				}
				else
					node = indexedNodes[index];

				if (head == index && list.Head == null)
					list.Head = node;

				if (tail == index && list.Tail == null)
					list.Tail = node;

				String dataValue = nodeMatch.Groups["Data"].Value;
				if (!dataValue.Equals(nullRef, StringComparison.Ordinal))
					node.Data = dataValue;

				String prevValue = nodeMatch.Groups["Prev"].Value;

				if (Int32.TryParse(prevValue, out var prev))
				{
					if (!indexedNodes.ContainsKey(prev))
					{
						node.Prev = new ListNode();
						indexedNodes.Add(prev, node.Prev);
					}
					else
						node.Prev = indexedNodes[prev];
				}

				String randValue = nodeMatch.Groups["Rand"].Value;

				if (Int32.TryParse(randValue, out var rand))
				{
					if (!indexedNodes.ContainsKey(rand))
					{
						node.Rand = new ListNode();
						indexedNodes.Add(rand, node.Rand);
					}
					else
						node.Rand = indexedNodes[rand];
				}

				String nextValue = nodeMatch.Groups["Next"].Value;

				if (Int32.TryParse(nextValue, out var next))
				{
					if (!indexedNodes.ContainsKey(next))
					{
						node.Next = new ListNode();
						indexedNodes.Add(next, node.Next);
					}
					else
						node.Next = indexedNodes[next];
				}
			}
		}

		public static String SerializeToString(this ListRand list)
		{
			Dictionary<ListNode, Int32> indexedNodes = new Dictionary<ListNode, Int32>(list.Count);
			Stack<ListNode> nodesToProcess = new Stack<ListNode>();
			Int32 currentIndex = 0;
			Int32? index;
			Boolean isNew;
			StringBuilder builder = new StringBuilder();

			//Start List
			builder.AppendLine(lCurly);
			//List Count
			builder.Append(listCount).Append(list.Count.ToString()).AppendLine(quoteComma);
			//List Head
			var indexHead = AddIndexedNode(ref indexedNodes, ref currentIndex, ref list.Head, out isNew);
			if (indexHead == null)
				builder.Append(listHead).Append(nullRef).AppendLine(quoteComma);
			else
				builder.Append(listHead).Append(indexHead).AppendLine(quoteComma);
			//List Tail
			var indexTail = AddIndexedNode(ref indexedNodes, ref currentIndex, ref list.Tail, out isNew);
			if (indexTail == null)
				builder.Append(listTail).Append(nullRef).AppendLine(quoteComma);
			else
				builder.Append(listTail).Append(indexTail).AppendLine(quoteComma);

			//Start Nodes
			builder.Append(listNodes);

            //TODO: Assume that Head and Tail are always reference to first/last list node and == null when list is empty
            if (list.Head != null)
			{
				if (isNew)
					nodesToProcess.Push(list.Tail);
				nodesToProcess.Push(list.Head);

				//Start Nodes
				builder.AppendLine(lCurly);
				builder.AppendLine(nodeHeader);

				while (true)
				{
					ListNode currentNode = nodesToProcess.Pop();

					//Start Node
					builder.AppendLine(nodeStart);
					//Node Index
					index = AddIndexedNode(ref indexedNodes, ref currentIndex, ref currentNode, out isNew);
					builder.Append(nodeIndex).Append(index).AppendLine(quoteComma);
					//Node Data
					builder.Append(nodeData).Append(currentNode.Data ?? nullRef).AppendLine(quoteComma);
					//Node Previous
					index = AddIndexedNode(ref indexedNodes, ref currentIndex, ref currentNode.Prev, out isNew);
					builder.Append(nodePrev);
					if (index == null)
						builder.Append(nullRef);
					else
					{
						if (isNew)
							nodesToProcess.Push(currentNode.Prev);
						builder.Append(index);
					}
					builder.AppendLine(quoteComma);
					//Node Random
					index = AddIndexedNode(ref indexedNodes, ref currentIndex, ref currentNode.Rand, out isNew);
					builder.Append(nodeRand);
					if (index == null)
						builder.Append(nullRef);
					else
					{
						if (isNew)
							nodesToProcess.Push(currentNode.Rand);
						builder.Append(index);
					}
					builder.AppendLine(quoteComma);
					//Node Next
					index = AddIndexedNode(ref indexedNodes, ref currentIndex, ref currentNode.Next, out isNew);
					builder.Append(nodeNext);
					if (index == null)
						builder.Append(nullRef);
					else
					{
						if (isNew)
							nodesToProcess.Push(currentNode.Next);
						builder.Append(index);
					}
					builder.AppendLine(quote);
					//End Node
					if (nodesToProcess.Count != 0)
						builder.Append(tab3).AppendLine(rCurlyComma);
					else
					{
						builder.Append(tab3).AppendLine(rCurly);
						break;
					}
				}

				//End Nodes
				builder.Append(tab).Append(tab).AppendLine(rSquare);
				builder.Append(tab).AppendLine(rCurly);
			}
			else
			{
				//End Nodes
				builder.Append(quote).Append(nullRef).AppendLine(quote);
			}

			//End List
			builder.Append(rCurly);

			return builder.ToString();
		}


		//TODO: Find appropriate name
		private static Int32? AddIndexedNode(ref Dictionary<ListNode, Int32> indexedNodes, ref Int32 currentIndex, ref ListNode node, out Boolean isNew)
		{
			Int32? index;
			isNew = false;

			if (node == null)
				index = null;
			else
			{
				if (indexedNodes.ContainsKey(node))
					index = indexedNodes[node];
				else
				{
					currentIndex++;
					indexedNodes.Add(node, currentIndex);
					index = currentIndex;
					isNew = true;
				}
			}

			return index;
		}
	}
}