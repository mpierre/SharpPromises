﻿using System;
using System.Collections.Generic;

namespace SharpPromises
{
    public class Deferred<T> : Promise<T>
    {
        private List<Action<T>> dones = new List<Action<T>>();
        private List<Action<Exception>> fails = new List<Action<Exception>>();
        private T Result;
        private Exception Error;

		public enum PromiseStatus { PENDING, RESOLVED, REJECTED };

		private PromiseStatus status = PromiseStatus.PENDING;
		public PromiseStatus Status { get { return status; } set { status = value; } }

        public Promise<E> Then<E>(Func<T, E> func)
        {
            var deferred = new Deferred<E>();
            this.Done(v => deferred.Resolve(func(v)));
            return deferred.Promise();
        }

        public Promise<E> Then<E>(Func<T, Promise<E>> func)
        {
            var deferred = new Deferred<E>();
			this.Done(v => {
				func(v)
                    .Done(a => deferred.Resolve(a))
                    .Fail(e => deferred.Reject(e));
            });
            return deferred.Promise();
        }

        public void Resolve(T value)
        {
			if (Status != PromiseStatus.PENDING) { return; }
            
            Result = value;
			Status = PromiseStatus.RESOLVED;
			foreach (Action<T> func in dones) {
                func(Result);
            }
        }

        public void Reject(Exception error)
		{
			if (Status != PromiseStatus.PENDING) { return; }

            Error = error;
			Status = PromiseStatus.REJECTED;
			foreach (Action<Exception> func in fails) {
				func(Error);
            }
        }

        public Promise<T> Done(Action<T> func)
        {
			if (func != null) {
				if (Status == PromiseStatus.RESOLVED) {
                    func(Result);
				} else {
                    dones.Add(func);
                }
            }
            return this;
        }

        public Promise<T> Fail(Action<Exception> func)
        {
            if (func != null)
            {
				if (Status == PromiseStatus.REJECTED) {
                    func(Error);
				} else {
                    fails.Add(func);
                }
            }
            return this;
        }

        public Promise<T> Always(Action<bool, T, Exception> func)
        {
            return this
                .Done(v => func(true, v, null))
				.Fail(e => func(false, default(T), e));
        }

        public Promise<E> Always<E>(Func<bool, T, Exception, Promise<E>> func)
        {
            var deferred = new Deferred<E>();
			this.Done(v => {
                    func(true, v, null)
						.Done(a => deferred.Resolve(a))
                        .Fail(e => deferred.Reject(e));
                })
				.Fail(ex => {
                    func(false, default(T), ex)
                        .Done(a => deferred.Resolve(a))
                        .Fail(e => deferred.Reject(e));
                });
            return deferred.Promise();
        }

        public Promise<T> Promise()
        {
            return this;
        }
    }
}
