using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Knapsack : MonoBehaviour
{
	public int[][] M;
	public Item[] arr;
	public int capacity;
	public List<string> visualArray;
	private void Start()
    {
		arr = new Item[] { new Item(12, 12), new Item(2, 2), new Item(1, 2), new Item(1, 1), new Item(4, 10) };
		capacity = 15;// capacity
		M = new int[capacity + 1][];
		/*
		Item[] arr = new Item[] { new Item(12, 12), new Item(2, 2), new Item(1, 2), new Item(1, 1), new Item(4, 10) };
		int c = 15;// capacity
		process(arr, c);
		*/
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < M[0].Length; i++)
            {
				M[0][i] = 0;
            }
		}
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
			for (var i = 1; i < arr.Length; i++)
			{
				M[i] = new int[capacity];
				System.Array.Fill(M[i], -99);
			}
		}
    }

    // 0-1 Knapsack Problem (Dynamic Programming)
    void process(Item[] arr, int c)
	{
		ver_Recursive(arr, c);
	}

	long start_time;
	Stopwatch stopwatch;
	void timer_start()
	{
		stopwatch = new Stopwatch();
		stopwatch.Start();
	}

	void timer_end_print()
	{
		stopwatch.Stop();
		print(stopwatch.ElapsedMilliseconds);
	}

	// -------------- -------------- --------------

	static int[][] memory;

	void ver_Recursive(Item[] arr, int capacity)
	{
		timer_start();
		int result = putItem(arr, arr.Length, capacity);
		print(result);
		timer_end_print();
	}

	// n is number of item; in each call, we consider the last item (n-1)
	// return is v
	int putItem(Item[] arr_full, int number, int capacity)
	{
		int value = 0;
		if (number == 0 || capacity == 0)
		{// no item left OR capacity out
			value = 0;
			print("There's no Item left! Or Maybe you're Run out of Capacity!");
		}
		else
		{
			Item thisitem = arr_full[number - 1];
			if (thisitem.weight > capacity)
			{ // item is too big
				value = putItem(arr_full, number - 1, capacity); // skip this item
			}
			else
			{// this item can be added
				int v_if_notput = putItem(arr_full, number - 1, capacity);// if not put this item
				int v_if_put = thisitem.value + putItem(arr_full, number - 1, capacity - thisitem.weight);// if put this item
				value = Mathf.Max(v_if_notput, v_if_put);
			}
			//		??????????
		}
		return value;
	}

	public class Item
	{
		public int weight = 0;
		public int value = 0;

		public Item(int w, int v)
		{
			weight = w;
			value = v;
		}
	}

}