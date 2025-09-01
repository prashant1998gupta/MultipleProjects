using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //int[] nums = { 3, 1, 4, 2 ,5 , 1 , -1 , 1 , 0 };
        // Debug.Log($"Find132Pattern(nums) {Find132Pattern(nums)}");  // Output: True
        // FindAll132Patterns(nums);  // Output: True
        //FindAll132Patterns1(nums);  // Output: True
        // FindAll132Patterns2(nums);  // Output: True
        //PrintHeart(6);
        //FizzBuzz(15);

        // (int i , int j) = TwoSum(new List<int> { 2, 7, 2, 20 }, 27);
        // Debug.Log($"this is i {i} and this is j {j}");

        // string reverseStrign = ReverseString("12345678");
        // Debug.Log($"reverseStrign {reverseStrign}");

        //bool isPalindromeCheck = PalindromeCheck("daaiiiaad");
        // Debug.Log($"reverseStrign {isPalindromeCheck}");

        //int longestSubArray = LongestSubArray("daaiiiaafsdfdsssdfsfsdfssfsdssfsdfsdwgfgdffhasslkfafioewtu00fasdiifjoi0weqoiijdfsaffjw80e089w0ijsafsdjalk;d");
        //Debug.Log($"LongestSubArray {longestSubArray}");

        //bool isValid = IsValid("{(){}{}[]{}()}");
        //Debug.Log($"isValid {isValid}");

        //bool isValidAnagram = IsAnagram("{(){}{}[]{}()}" , "}()}{(){}{}[]{");
        //bool isValidAnagram = IsAnagram("hard" , "ardha");
        //Debug.Log($"isValid {isValidAnagram}");
    }

    public bool Find132Pattern(int[] nums)
    {
        if (nums.Length < 3) return false; // A 132 pattern requires at least 3 elements.

        Stack<int> stack = new Stack<int>();
        int third = int.MinValue; // Represents the nums[k] value (largest valid for 132 pattern)

        // Traverse the array from right to left
        for (int i = nums.Length - 1; i >= 0; i--)
        {
            if (nums[i] < third)
            {
                return true; // Found nums[i] < nums[k] < nums[j]
            }
            while (stack.Count > 0 && nums[i] > stack.Peek())
            {
                third = stack.Pop(); // Update third to the highest valid nums[k]
            }
            stack.Push(nums[i]); // Push current element as potential nums[j]
        }

        return false;
    }

    public void FindAll132Patterns(int[] nums)
    {
        int n = nums.Length;
        bool found = false;

        for (int i = 0; i < n - 2; i++)
        {
            for (int j = i + 1; j < n - 1; j++)
            {
                for (int k = j + 1; k < n; k++)
                {
                    if (nums[i] < nums[k] && nums[k] < nums[j])
                    {
                        Console.WriteLine();

                        Debug.Log($"({nums[i]}, {nums[j]}, {nums[k]})");
                        found = true;
                    }
                }
            }
        }

        if (!found)
        {
            Debug.Log("No 132 pattern found.");
        }
    }

    public void FindAll132Patterns1(int[] nums)
    {
        int n = nums.Length;
        if (n < 3)
        {
            Debug.Log("No 132 pattern found.");
            return;
        }

        bool found = false;

        for (int j = 1; j < n - 1; j++)
        {
            int min_i = int.MaxValue;
            // Find the smallest nums[i] before j
            for (int i = 0; i < j; i++)
            {
                min_i = Math.Min(min_i, nums[i]);
            }
            // Look for nums[k] > min_i and < nums[j]
            for (int k = j + 1; k < n; k++)
            {
                if (min_i < nums[k] && nums[k] < nums[j])
                {
                    Debug.Log($"({min_i}, {nums[j]}, {nums[k]})");
                    found = true;
                }
            }
        }

        if (!found)
        {
            Debug.Log("No 132 pattern found.");
        }
    }

    public void FindAll132Patterns2(int[] nums)
    {
        int n = nums.Length;
        if (n < 3)
        {
            Debug.Log("No 132 pattern found.");
            return;
        }

        bool found = false;
        HashSet<int> leftSet = new HashSet<int>(); // Store nums[i] dynamically

        for (int j = 1; j < n - 1; j++)
        {
            leftSet.Add(nums[j - 1]); // Update possible `nums[i]` values

            foreach (int num_i in leftSet)
            {
                if (num_i < nums[j])
                { // Find `nums[k]`
                    for (int k = j + 1; k < n; k++)
                    {
                        if (num_i < nums[k] && nums[k] < nums[j])
                        {
                            Console.WriteLine($"({num_i}, {nums[j]}, {nums[k]})");
                            found = true;
                        }
                    }
                }
            }
        }

        if (!found)
        {
            Debug.Log("No 132 pattern found.");
        }
    }

    /*
    Right-Angled Triangle
    */

    public void RightAngledTriangle(int n)
    {
        string s = "";

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                s = s + "*";
            }
            Debug.Log($"{s}\n");
            s = "";
        }
    }

    public void InvertedRightAngledTriangle(int n)
    {
        string s = "";

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n - i; j++)
            {
                s = s + "*";
            }
            Debug.Log($"{s}\n");
            s = "";
        }
    }

    public void PyramidPattern(int n)
    {

        for (int i = 0; i < n; i++)
        {
            string s = "";
            string spaces = new string(' ', n - (i + 1));

            for (int j = 0; j < ((i + 1) * 2) - 1; j++)
            {
                s = s + $"{j + 1}";
            }
            Debug.Log($"{spaces}{s}");
        }

        for (int i = 1; i <= n; i++)
        {
            string spaces1 = new string(' ', n - i);
            string stars = new string('*', 2 * i - 1);
            Debug.Log(spaces1 + stars);
        }
    }

    public void InvertedPattern(int n)
    {

        for (int i = 0; i < n; i++)
        {
            string s = "";
            string spaces = new string(' ', i);

            for (int j = 0; j < (n * 2) - (((i + 1) * 2) - 1); j++)
            {
                s = s + $"{j + 1}";
            }
            Debug.Log($"{spaces}{s}");
        }

        for (int i = 1; i <= n; i++)
        {
            string spaces1 = new string(' ', i - 1);
            string stars = new string('*', (n * 2) - ((i * 2) - 1));
            Debug.Log(spaces1 + stars);
        }
    }

    public void DiamondPattern(int n)
    {

        for (int i = 0; i < n; i++)
        {
            string s = "";
            string spaces = "";

            if (i < n / 2)
            {
                spaces = new string(' ', n - (i + 1));

                for (int j = 0; j < ((i + 1) * 2) - 1; j++)
                {
                    s = s + $"{j + 1}";
                }
            }
            else
            {
                spaces = new string(' ', i);

                for (int j = 0; j < (n * 2) - (((i + 1) * 2) - 1); j++)
                {
                    s = s + $"{j + 1}";
                }
            }

            Debug.Log($"{spaces}{s}");
        }

    }

    public void HollowPattern(int n)
    {

        for (int i = 0; i < n; i++)
        {
            string s = "";
            string spaces = new string(' ', n - (i + 1));

            for (int j = 0; j < ((i + 1) * 2) - 1; j++)
            {
                if (i == 0 || i == n - 1)
                {
                    s = s + $"*";
                }
                else
                {
                    if (j == 0)
                    {
                        s = s + $"*";
                    }

                    if (j != 0 && j + 1 != ((i + 1) * 2) - 1)
                    {
                        //s = s + $"{j + 1}";
                        s = s + $"  ";
                    }

                    if (j + 1 == ((i + 1) * 2) - 1)
                    {
                        s = s + $"*";
                    }

                }
            }
            Debug.Log($"{spaces}{s}");
        }
    }

    public void HollowInvertedPattern(int n)
    {

        for (int i = 0; i < n; i++)
        {
            string s = "";
            string spaces = new string(' ', i);

            for (int j = 0; j < (n * 2) - (((i + 1) * 2) - 1); j++)
            {
                if (i == 0 || i == n - 1)
                {
                    s = s + $"*";
                }
                else
                {
                    if (j == 0)
                    {
                        s = s + $"*";
                    }

                    if (j != 0 && j + 1 != (n * 2) - (((i + 1) * 2) - 1))
                    {
                        //s = s + $"{j + 1}";
                        s = s + $" ";
                    }

                    if (j + 1 == (n * 2) - (((i + 1) * 2) - 1))
                    {
                        s = s + $"*";
                    }

                }
            }
            Debug.Log($"{spaces}{s}");
        }
    }

    public void SquarePattern(int n)
    {
        for (int i = 0; i < n; i++)
        {
            string s = "";

            for (int j = 0; j < n; j++)
            {
                s = s + "*";
            }

            Debug.Log($"{s}");
        }
    }

    public void SquareHollowPattern(int n)
    {
        for (int i = 0; i < n; i++)
        {
            string s = "";

            for (int j = 0; j < n; j++)
            {
                if (i == 0 || i == n - 1)
                {
                    s = s + $"*";
                }
                else
                {
                    if (j == 0 || j == n - 1)
                    {
                        s = s + "*";
                    }
                    if (j != 0 && j != n - 1)
                    {
                        s = s + $"{j + 1}";
                    }
                }

            }

            Debug.Log($"{s}");
        }
    }

    public void PascalsTriangle(int n)
    {

        for (int i = 1; i <= n; i++)
        {
            string result = "";
            string spaces = new string(' ', n - (i));

            result += spaces;

            int number = 1;

            for (int j = 1; j <= i; j++)
            {
                result += number + " ";
                number = number * (i - j) / (j);
            }

            Debug.Log($"{spaces}{result} ");
        }
    }

    public void NumberPascals(int n)
    {

        for (int i = 1; i <= n; i++)
        {
            string spaces = new string(' ', n - (i));
            string s = "";

            for (int j = 1; j <= i; j++)
            {
                s = s + $"{i} ";
            }

            Debug.Log($"{spaces}{s}");
        }
    }

    public void HollowNumberPyramid(int n)
    {

        for (int i = 1; i <= n; i++)
        {
            string spaces = new string(' ', n - (i));
            string s = "";

            for (int j = 1; j <= i * 2 - 1; j++)
            {
                if (i == 1 || i == n)
                {
                    s = s + $"{i}";
                }
                else
                {
                    if (j == 1 || j == i * 2 - 1)
                    {
                        s = s + $"{i}";
                    }
                    else
                    {
                        s = s + $" ";
                    }
                }
            }
            Debug.Log($"{spaces}{s}");
        }
    }

    public void DiamondOfNumbers(int n)
    {
        for (int i = 1; i <= n; i++)
        {

            string s = "";

            if (i <= (n / 2) + 1)
            {
                string spaces = new string(' ', (n / 2) + 1 - (i));

                for (int j = 1; j <= i * 2 - 1; j++)
                {
                    if (i == 1 || i == (n / 2) + 1)
                    {
                        s = s + $"{i}";
                    }
                    else
                    {
                        if (j == 1 || j == i * 2 - 1)
                        {
                            s = s + $"{i}";
                        }
                        else
                        {
                            s = s + $" ";
                        }
                    }
                }
                Debug.Log($"{spaces}{s}");
            }
            else
            {
                string spaces = new string(' ', i);


                for (int j = 1; j <= (n * 2) - (((i - 1) * 2) + 1); j++)
                {
                    //Debug.Log($"(n * 2) - (((i + 1) * 2) - 1) {(n * 2) - (((i + 1) * 2) - 1)}");
                    {
                        if (j == 1 || j == (n * 2) - (((i - 1) * 2) + 1))
                        {
                            s = s + $"{i}";
                        }
                        else
                        {
                            s = s + $" ";
                        }
                    }
                }
                Debug.Log($"{spaces}{s}");
            }
        }
    }     // not achieved

    public void MirrorImageRightTriangle(int n)
    {

        for (int i = 1; i <= n; i++)
        {
            string spaces = new string(' ', n - (i));
            string s = "";

            for (int j = 1; j <= i; j++)
            {
                s = s + $"{j} ";
            }

            Debug.Log($"{spaces}{s}");
        }
    }

    public void MirroredPascalTriangle(int n)
    {

        for (int i = 1; i <= n; i++)
        {
            string result = "";
            string spaces = new string(' ', n -i);

            result += spaces;

            int number = 1;

            for (int j = 1; j <= i; j++)
            {
                result += number;
                number = number * (i - j) / (j);
            }

            Debug.Log($"{spaces}{result} ");
        }
    }

    public void HollowNumberSquare(int n)
    {
        for (int i = 1; i <= n; i++)
        {
            string s = "";

            for (int j = 1;j <= n; j++)
            {
                if (i == 1 || i == n)
                {
                  s = s + $"1";
                }
                else
                {
                    if (j == 1 || j == n)
                    {
                        s = s + $"1";
                    }
                    else
                    {
                        s = s + $"  ";
                    }
                }
            }

            Debug.Log($"{s}");
        }
    }

    public void NumberTriangle(int n)
    {
        int num = 0;

        for (int i = 1; i <= n; i++)
        {
            string s = "";

            for (int j = 1; j <= i; j++)
            {
                num = num + 1;
                s = s + $"{num} ";
            }

            Debug.Log($"{s}");
        }
    }

    public void ReverseNumberPyramid(int n)
    {
       
        for (int i = 1; i <= n; i++)
        {
            string s = "";

            for (int j = 1; j <= n - (i-1); j++)
            {
                s = s + $"{j} ";
            }

            Debug.Log($"{s}");
        }
    }

    public void AlphabetPyramid(int n)
    {
        for (int i = 0; i < n; i++)
        {
            string space = new string(' ', n - i);

            string line = "";

            /* string s = "";

             for (int j = 1; j <= i; j++)
             {
                 s += $"A ";
             }*/

            for (char c = 'A'; c <= 'A' + i; c++)
            {
                line += c;
            }

            // Reverse sequence
            for (char c = (char)('A' + i - 1); c >= 'A'; c--)
            {
                line += c;
            }

            Debug.Log(space + line);
        }
    }

    public void nvertedAlphabetPyramid(int n)
    {
        for (int i = 1; i <= n; i++)
        {
            string space = new string(' ', i-1);
            string line = "";

            for (char c = 'A'; c <= 'A' + n-i; c++)
            {
                line += c;

            }

            // Reverse sequence
            for (char c = (char)('A' + n - i-1); c >= 'A'; c--)
            {
                line += c;
            }

            Debug.Log(space + line);
        }
    }

    public void CharacterTriangle(int n)
    {
        char alpha = 'A';

        for (int i = 0; i <= n; i++)
        {
            string line = "";
            for (char c = 'A'; c <= 'A' + i; c++)
            {
                line += alpha;
            }
            alpha++;
            Debug.Log(line);
        }
    }

    void PrintHeart(int n)
    {
        int size = n; // Adjust size of the heart
        string heart = "";

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j <= 4 * size; j++)
            {
                double d1 = Mathf.Sqrt(Mathf.Pow(i - size, 2) + Mathf.Pow(j - size, 2));
                double d2 = Mathf.Sqrt(Mathf.Pow(i - size, 2) + Mathf.Pow(j - 3 * size, 2));

                if ((d1 < size + 0.5 || d2 < size + 0.5) || (i >= size && (j > (i - size) * 2 && j < 4 * size - (i - size) * 2)))
                    heart += "*";
                else
                    heart += " ";
            }
            heart += "\n";
        }

        Debug.Log(heart);
    }

    (int , int)  TowSum(List<int> nums , int target)
    {
        for(int i = 0;i < nums.Count;i++)
        {
            for (int j = 0; j < nums.Count; j++)
            {
                if (i == j) continue;

                if (nums[i] + nums[j] == target)
                    return(i , j);
            }
        }

        return (-1 , -1);
    }

    (int, int) TwoSum(List<int> nums, int target)
    {
        Dictionary<int, int> numMap = new Dictionary<int, int>();

        for (int i = 0; i < nums.Count; i++)
        {
            int complement = target - nums[i];

            Debug.Log($"complement {complement}");

            if (numMap.ContainsKey(complement))
                return (numMap[complement], i);

            numMap[nums[i]] = i; // Store the number with its index


        }

        return (-1, -1); // If no solution found
    }

    string ReverseString(string s)
    {

        char[] chars = s.ToCharArray();  // Convert string to char array

        int left = 0;
        int right = chars.Length - 1;

        // first method 
        for (int i = 0; i < s.Length/2; i++)
        {
            char temp = chars[i];
            chars[i] = chars[s.Length - i - 1];
            chars[s.Length - i - 1] = temp;
        }

        // second method
        while (left < right)
        {
            // Swap characters
            char temp = chars[left];
            chars[left] = chars[right];
            chars[right] = temp;

            left++;
            right--;
        }


        return new string(chars);  // Convert back to string
    }

    bool PalindromeCheck1(string s)  // in this reverse string and compaire with original one 
    {
        char[] chars = s.ToCharArray();  // Convert string to char array

        for (int i = 0; i < s.Length / 2; i++)
        {
            char temp = chars[i];
            chars[i] = chars[s.Length - i - 1];
            chars[s.Length - i - 1] = temp;
        }

        if (s.Equals(new string(chars)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool PalindromeCheck(string s)  // in this not reverse string but only check char if char match than return true if not than false 
    {
        for (int i = 0; i < s.Length / 2; i++)
        {
            if (s[i] != s[s.Length - i - 1])
            {
                return false;
            }
        }
        return true;
    }

    //Longest Substring Without Repeating Characters

    int LongestSubArray(string s)
    {
        HashSet<char> set = new HashSet<char>();
        int left = 0, maxLength = 0;

        for (int right = 0; right < s.Length; right++)
        {
            while (set.Contains(s[right]))
            {
                set.Remove(s[left]);
                left++;
            }

            set.Add(s[right]);
            maxLength = Math.Max(maxLength, right - left + 1);
        }

        return maxLength;
    }

    void MoveZeroes(int[] nums)
    {
        int index = 0; // Pointer for the position to place non-zero elements

        // First, move all non-zero elements to the front
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] != 0)
            {
                nums[index] = nums[i];
                index++;
            }
        }

        // Fill the remaining positions with zeroes
        while (index < nums.Length)
        {
            nums[index] = 0;
            index++;
        }
    }

    int StrStr(string haystack, string needle)
    {
        // If the needle is an empty string, return 0 (as per standard behavior)
        if (needle == "") return 0;

        // Loop through haystack until there's room for the needle
        for (int i = 0; i <= haystack.Length - needle.Length; i++)
        {
            // Get a substring from haystack that's the same length as needle
            string part = haystack.Substring(i, needle.Length);

            // If this part matches needle, return the index
            if (part == needle)
            {
                return i;
            }
        }

        // If we reach here, needle wasn't found
        return -1;
    }

    //Check if Two Strings Are Anagrams – Given two strings, check if they are anagrams.
    bool IsAnagram(string s, string t)
    {
        if (s.Length != t.Length)
            return false;

        int[] counts = new int[26]; // for lowercase a-z

        for (int i = 0; i < s.Length; i++)
        {
            counts[s[i] - 'a']++;  // add count for s
            counts[t[i] - 'a']--;  // subtract count for t
        }

        // If all counts are zero, then they are anagrams
        foreach (int count in counts)
        {
            if (count != 0)
                return false;
        }

        return true;
    }

    //FizzBuzz – Print numbers from 1 to n, replacing multiples of 3 with "Fizz" and 5 with "Buzz".
    /* Print numbers from 1 to n, but:

 For multiples of 3, print "Fizz" instead of the number

 For multiples of 5, print "Buzz"

 For numbers that are multiples of both 3 and 5, print "FizzBuzz"*/

    void FizzBuzz(int n)
    {
        for (int i = 1; i <= n; i++)
        {
            if (i % 3 == 0 && i % 5 == 0)
            {
                Debug.Log("FizzBuzz");
            }
            else if (i % 3 == 0)
            {
                Debug.Log("Fizz");
            }
            else if (i % 5 == 0)
            {
                Debug.Log("Buzz");
            }
            else
            {
                Debug.Log(i);
            }
        }
    }


    //Valid Parentheses – Check if a string with brackets is valid.
    /*Problem Statement:
    Given a string containing only brackets:
    '(', ')', '{', '}', '[' and ']'
    Check if the string is valid, meaning:

    Every opening bracket must have a corresponding closing bracket.

    Brackets must be closed in the correct order.*/

    bool IsValid(string s)
    {
        Stack<char> stack = new Stack<char>();

        foreach (char c in s)
        {
            if (c == '(' || c == '{' || c == '[')
            {
                stack.Push(c); // push opening brackets
            }
            else
            {
                if (stack.Count == 0) return false;

                char top = stack.Pop();

                if ((c == ')' && top != '(') ||
                    (c == '}' && top != '{') ||
                    (c == ']' && top != '['))
                {
                    return false;
                }
            }
        }

        return stack.Count == 0; // stack should be empty if all brackets matched
    }


    //Implement Queue using Stacks – Use two stacks to implement a queue.
    /*We use two stacks:

stackIn for enqueue (push)

stackOut for dequeue (pop/peek)

Whenever we dequeue or peek and stackOut is empty, we move all elements from stackIn to stackOut.*/

    public class MyQueue
    {
        private Stack<int> stackIn = new Stack<int>();
        private Stack<int> stackOut = new Stack<int>();

        // Enqueue: Add item to queue
        public void Push(int x)
        {
            stackIn.Push(x);
        }

        // Dequeue: Remove item from front of queue
        public int Pop()
        {
            if (stackOut.Count == 0)
            {
                while (stackIn.Count > 0)
                {
                    stackOut.Push(stackIn.Pop());
                }
            }

            return stackOut.Pop();
        }

        // Peek: Get front item without removing it
        public int Peek()
        {
            if (stackOut.Count == 0)
            {
                while (stackIn.Count > 0)
                {
                    stackOut.Push(stackIn.Pop());
                }
            }

            return stackOut.Peek();
        }

        // Check if queue is empty
        public bool Empty()
        {
            return stackIn.Count == 0 && stackOut.Count == 0;
        }
    }
}
