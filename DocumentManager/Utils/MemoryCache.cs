using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web;

namespace DocumentManager.Utils {
	public sealed class MemoryCache<T> where T : class {
		public delegate T CacheStorageRefresher();

		private readonly CacheStorageRefresher cacheStorageRefresher;
		private readonly int ttlMS;
		private int lastRefreshTime;
		private T storage;
		private readonly CommonReadRareWriteLock sync;

		public MemoryCache(CacheStorageRefresher cacheStorageRefresher, int ttlMS = -1) {
			this.cacheStorageRefresher = cacheStorageRefresher;
			this.ttlMS = ttlMS;
			sync = new CommonReadRareWriteLock();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Refresh() => Refresh(cacheStorageRefresher());

		public void Refresh(T storage) {
			sync.EnterWriteLock();
			try {
				this.storage = storage;
				lastRefreshTime = Environment.TickCount;
			} finally {
				sync.ExitWriteLock();
			}
		}

		public T StartReading() {
			if (storage == null) {
				// Prevent multiple concurrent refreshes during first time initializations
				lock (sync) {
					if (storage == null) {
						try {
							Refresh();
						} catch {
							// Just ignore...
						}
					}
				}
			}
			if (ttlMS <= 0 || (Environment.TickCount - lastRefreshTime) <= ttlMS) {
				sync.EnterReadLock();
				return storage;
			}
			try {
				Refresh();
			} catch {
				// Just ignore...
			}
			sync.EnterReadLock();
			return storage;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FinishReading() => sync.ExitReadLock();
	}
}
