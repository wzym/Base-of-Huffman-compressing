using System;
using System.Collections.Generic;
using System.Text;

namespace Stepik02
{
    internal class Program
    {
        public static void Main()
        {
            var stringToEncode = Console.ReadLine();
            if (string.IsNullOrEmpty(stringToEncode)) return;
            var frequencies = CountAndGetFrequencies(stringToEncode);
            
            var myHeap = new MyBinMinHeap(26);
            foreach (var charFrequency in frequencies)
                myHeap.Add(new CharFreq(charFrequency.Key, charFrequency.Value));

            while (myHeap.Count > 1)
            {
                var curMin = myHeap.GetMin();
                var curMax = myHeap.GetMin();
                var newParent = new CharFreq('Ё', curMin.Frequency + curMax.Frequency)
                {
                    LeftChild = curMin, RightChild = curMax
                };
                myHeap.Add(newParent);
            }

            var newRoot = myHeap.GetMin();
            var codes = new Dictionary<char, string>();

            CollectAllCodes(newRoot, "", codes);

            var encoded = Encode(stringToEncode, codes);
            PrintResult(codes, encoded);
        }

        private static string Encode(string stringToEncode, IReadOnlyDictionary<char, string> codes)
        {
            var sb = new StringBuilder();
            foreach (var symbol in stringToEncode)
            {
                sb.Append(codes[symbol]);
            }

            return sb.ToString();
        }

        private static void CollectAllCodes(CharFreq node, string currentCode, IDictionary<char, string> codes)
        {
            if (node.Symbol != 'Ё') codes.Add(node.Symbol, currentCode);
            if (node.LeftChild == null && node.RightChild == null) return;
            if (node.LeftChild != null) CollectAllCodes(node.LeftChild, currentCode + '0', codes);
            if (node.RightChild != null) CollectAllCodes(node.RightChild, currentCode + '1', codes);
        }

        private static void PrintResult(Dictionary<char,string> codes, string encoded)
        {
            Console.WriteLine(codes.Count + " " + encoded.Length);
            var sb = new StringBuilder();
            foreach (var code in codes)
                sb.AppendLine(code.Key + ": " + code.Value);
            Console.WriteLine(sb);
            Console.WriteLine(encoded);
        }

        private static Dictionary<char, int> CountAndGetFrequencies(string stringToEncode)
        {
            var frequencies = new Dictionary<char, int>();
            foreach (var symbol in stringToEncode)
                if (frequencies.ContainsKey(symbol))
                    frequencies[symbol]++;
                else
                    frequencies.Add(symbol, 1);
            return frequencies;
        }

        private class MyBinMinHeap
        {
            private readonly CharFreq[] _body;
            private int _indexOfLastElement;
            internal int Count => _indexOfLastElement + 1;

            public MyBinMinHeap(int sizeOfHeap)
            {
                _body = new CharFreq[sizeOfHeap];
                _indexOfLastElement = -1; 
            }

            internal void Add(CharFreq elementToAdd)
            {
                _body[++_indexOfLastElement] = elementToAdd;
                RaiseOut(_indexOfLastElement);
            }

            private void RaiseOut(int elemToRaiseIndex)
            {
                var currentParenInd = GetParentIndex(elemToRaiseIndex);
                while (currentParenInd >= 0 && _body[currentParenInd].Frequency > _body[elemToRaiseIndex].Frequency)
                {
                    Swap(elemToRaiseIndex, currentParenInd);
                    elemToRaiseIndex = currentParenInd;
                    currentParenInd = GetParentIndex(elemToRaiseIndex);
                }
            }
                   
            internal CharFreq GetMin()
            {
                var maxToReturn = _body[0];
                RemoveElement(0);
                return maxToReturn;
            }
            
            private void RemoveElement(int indexOfElement)
            {
                Swap(indexOfElement, _indexOfLastElement);
                _indexOfLastElement--;
                Drown(indexOfElement);
            }
                
            private void Drown(int elemToDrownIndex)
            {
                var maxChildIndex = GetMinChildIndex(elemToDrownIndex);
                while (maxChildIndex >= 0 && _body[maxChildIndex].Frequency < _body[elemToDrownIndex].Frequency)
                {
                    Swap(maxChildIndex, elemToDrownIndex);
                    elemToDrownIndex = maxChildIndex;
                    maxChildIndex = GetMinChildIndex(elemToDrownIndex);
                } 
            }

       
            private int GetMinChildIndex(int elemToDrownIndex)
            {
                var presumedLeftIndex = GetLeftChildIndex(elemToDrownIndex);
                if (presumedLeftIndex < 0 || presumedLeftIndex > _indexOfLastElement)
                    return -1;
                var presumedRightIndex = presumedLeftIndex + 1;
                if (presumedRightIndex > _indexOfLastElement) return presumedLeftIndex;
                return _body[presumedRightIndex].Frequency < _body[presumedLeftIndex].Frequency
                    ? presumedRightIndex : presumedLeftIndex;
            }

    
            private void Swap(int index1, int index2)
            {
                var buffer = _body[index1];
                _body[index1] = _body[index2];
                _body[index2] = buffer;
            }
            
            private static int GetParentIndex(int childIndex) => (childIndex - 1) / 2;
            private static int GetLeftChildIndex(int parentIndex) => parentIndex * 2 + 1;
        }

        private class CharFreq
        {
            internal readonly char Symbol;
            internal readonly int Frequency;
            internal CharFreq LeftChild { get; set; }
            internal CharFreq RightChild { get; set; }
            
            internal CharFreq(char symbol, int frequency)
            {
                Symbol = symbol;
                Frequency = frequency;
            }
        }
    }
}