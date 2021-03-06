﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Diagnostics;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	using System.Threading.Tasks;
	using System.Threading;
	public sealed partial class CollectionRecreateAction : CollectionAction
	{

		/// <summary> Execute this action</summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <remarks>
		/// This method is called when a new non-null collection is persisted
		/// or when an existing (non-null) collection is moved to a new owner
		/// </remarks>
		public override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			bool statsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopwatch = null;
			if (statsEnabled)
			{
				stopwatch = Stopwatch.StartNew();
			}
			IPersistentCollection collection = Collection;

			await (PreRecreateAsync(cancellationToken)).ConfigureAwait(false);

			var key = await (GetKeyAsync(cancellationToken)).ConfigureAwait(false);
			await (Persister.RecreateAsync(collection, key, Session, cancellationToken)).ConfigureAwait(false);

			var entry = Session.PersistenceContext.GetCollectionEntry(collection);
			// On collection create, the current key may refer a delayed post insert instance instead
			// of the actual identifier, that GetKey above should have resolved at that point. Update
			// it.
			entry.CurrentKey = key;
			entry.AfterAction(collection);

			await (EvictAsync(cancellationToken)).ConfigureAwait(false);

			await (PostRecreateAsync(cancellationToken)).ConfigureAwait(false);
			if (statsEnabled)
			{
				stopwatch.Stop();
				Session.Factory.StatisticsImplementor.RecreateCollection(Persister.Role, stopwatch.Elapsed);
			}
		}

		private async Task PreRecreateAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPreCollectionRecreateEventListener[] preListeners = Session.Listeners.PreCollectionRecreateEventListeners;
			if (preListeners.Length > 0)
			{
				PreCollectionRecreateEvent preEvent = new PreCollectionRecreateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < preListeners.Length; i++)
				{
					await (preListeners[i].OnPreRecreateCollectionAsync(preEvent, cancellationToken)).ConfigureAwait(false);
				}
			}
		}

		private async Task PostRecreateAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPostCollectionRecreateEventListener[] postListeners = Session.Listeners.PostCollectionRecreateEventListeners;
			if (postListeners.Length > 0)
			{
				PostCollectionRecreateEvent postEvent = new PostCollectionRecreateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < postListeners.Length; i++)
				{
					await (postListeners[i].OnPostRecreateCollectionAsync(postEvent, cancellationToken)).ConfigureAwait(false);
				}
			}
		}
	}
}
