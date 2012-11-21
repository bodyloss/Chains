Chains
======

Allows for the declaration of Func<Type, Type>'s that are then chained together and executed synchronously or asynchronously with error handling and type checking built in.
When execute asynchnously an event is fired when the execution of a chain has finished, the event contains the result or the error if one occured during excecution.

Usage
=====

You can take a look at the example project for a set of examples show-casing both the synchnous and asynchnous usage. 
A (not at all real world) exmaple chain to generate a random array of integers and calculate the average is as follows:

    Chain<int, double> chain = new Chain<int, double>()
      .Link<int>(x =>
      {
        // Generate random array of ints
        int[] nums = new int[x];
        Random rand = new Random();
        for (int i = 0; i < x; i++)
        {
          nums[i] = rand.Next(100);
        }
        return nums;
      })
      .Link<int[]>(x =>
      {
        // order the numbers (pointless but why not)
        return x.OrderBy(y => y).ToArray();
      })
      .Link<int[]>(x =>
      {
        // compute the average and return it
        double average = 0.0;
        for (int i = 0; i < x.Length; i++)
        {
          average += x[i];
        }
        return average / x.Length;
      });
    
    chain.Execute(10);

---

TODO
=====
* Add async methods following the async pattern (along-side events)
* Alter Chain so that a cast is not required, currently each Func<TIn, TOut> is wrapped in a Func<TIn, object> which casts TIn from object to TIn
* Add more control of how errors are handled, i.e. allow errors to be passed though the chain instead of stopping the execution so that links can handle the exception themselves
* Investigate methods to speed up execution of a chain
