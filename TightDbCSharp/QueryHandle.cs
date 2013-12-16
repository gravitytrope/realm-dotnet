﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TightDbCSharp
{
    //tables and tableviews will always return query c++ classes that must be unbound
    //all other query returning calls (on query itself) will return the same object as was called,
    //and therefore have been changed to void calls in c++ part of binding
    //so these handles always represent a qeury object that should be released when not used anymore
    //the C# binding methods on query simply return self to add the . nottation again
    //A query will be a child of whatever root its creator has as root (queries are usually created by tableviews and tables)
    public class QueryHandle:TightDbHandle
    {
        public QueryHandle(TightDbHandle root) : base(root)
        {
        }


        //acquire a TableView handle with the result And set Root in an atomic fashion 
        internal TableViewHandle QueryFindAll(long start, long end, long limit)
        {
            var tvHandle = TableViewHandle.RootedTableViewHandle(this);//same root as the query

            //At this point sh is invalid due to its handle being uninitialized, but the root is set correctly
            //a finalize at this point will not leak anything and the handle will not do anything

            //now, set the TableView handle...
            RuntimeHelpers.PrepareConstrainedRegions();//the following finally will run with no out-of-band exceptions
            try
            { }
            finally
            {
                tvHandle.SetHandle(UnsafeNativeMethods.QueryFindAll(this, start, end , limit));
            }//at this point we have atomically acquired a handle and also set the root correctly so it can be unbound correctly
            return tvHandle;
        }

        //acquire a TableView handle with the result And set Root in an atomic fashion 
        internal TableViewHandle QueryFindAll()
        {
            var tvHandle = TableViewHandle.RootedTableViewHandle(this);//same root as the query

            //At this point sh is invalid due to its handle being uninitialized, but the root is set correctly
            //a finalize at this point will not leak anything and the handle will not do anything

            //now, set the TableView handle...
            RuntimeHelpers.PrepareConstrainedRegions();//the following finally will run with no out-of-band exceptions
            try
            { }
            finally
            {
                tvHandle.SetHandle(UnsafeNativeMethods.QueryFindAll(this));
            }//at this point we have atomically acquired a handle and also set the root correctly so it can be unbound correctly
            return tvHandle;
        }

        protected override void Unbind()
        {
            UnsafeNativeMethods.QueryDelete(this);
        }
    }
}