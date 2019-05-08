using System.Threading;

namespace DocumentManager.Utils {
	public class CommonReadRareWriteLock {
		// This class is optimized for cases where writes are really slow
		// procedures, such as database accesses.
		// All the Sleep()'s were used for that reason. Should write times be
		// as fast as, or very close to, read times, the timeouts used in the
		// Sleep()'s could be decreased. In the best case, all Sleep()'s could
		// be replaced with Yield().

		// bits 31 ... 1 -> reader counter
		// bit 0 -> write flag
		// count must be volatile because it is changed by several threads
		private volatile int counter;

		public void EnterReadLock() {
			for (;;) {
				// Someone was already writing before us (here, since writes
				// are considered rare they have priority over reads)
				if ((counter & 1) != 0) {
					Thread.Sleep(10);
					continue;
				}

				int newCounter = Interlocked.Add(ref counter, 2);
				if ((newCounter & 1) != 0) {
					Interlocked.Add(ref counter, -2);
					// Someone was already writing before us (here, since writes
					// are considered rare they have priority over reads)
					Thread.Sleep(10);
					continue;
				}

				Interlocked.MemoryBarrier();
				return;
			}
		}

		public void ExitReadLock() {
			Interlocked.MemoryBarrier();
			Interlocked.Add(ref counter, -2);
		}

		public void EnterWriteLock() {
			for (;;) {
				int oldCounter;

				// Since Interlocked class does not have OR's,
				// we must implement it ourselves ;)
				for (;;) {
					oldCounter = counter;
					int newCounter = oldCounter | 1;
					if (Interlocked.CompareExchange(ref counter, newCounter, oldCounter) == oldCounter)
						break;
					Thread.Yield();
				}

				// Someone was already writing before us (here, since writes
				// are considered rare they have priority over reads)
				if ((oldCounter & 1) == 1) {
					Thread.Sleep(10);
					continue;
				}

				// Wait here while there is someone reading...
				// (writes have priority over reads, therefore we must not use continue)
				while (counter != 1)
					Thread.Yield();

				Interlocked.MemoryBarrier();
				return;
			}
		}

		public bool TryEnterWriteLock() {
			int oldCounter;

			// Since Interlocked class does not have OR's,
			// we must implement it ourselves ;)
			for (;;) {
				oldCounter = counter;
				int newCounter = oldCounter | 1;
				if (Interlocked.CompareExchange(ref counter, newCounter, oldCounter) == oldCounter)
					break;
				Thread.Yield();
			}

			// Someone was already writing before us (here, since writes
			// are considered rare they have priority over reads)
			if ((oldCounter & 1) == 1) {
				Thread.Sleep(5);
				Interlocked.MemoryBarrier();
				return false;
			}

			// Wait here while there is someone reading...
			// (writes have priority over reads, therefore we must not use continue)
			while (counter != 1)
				Thread.Yield();

			Interlocked.MemoryBarrier();
			return true;
		}

		public void ExitWriteLock() {
			Interlocked.MemoryBarrier();
			Interlocked.Decrement(ref counter);
		}
	}
}
