﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.Diagnostics.Tracing;
using System;

namespace Microsoft.Diagnostics.EventListeners
{
    public abstract class BufferingEventListener: EventListener
    {
        protected ConcurrentEventSender<EventData> Sender { get; set; }

        public bool ApproachingBufferCapacity
        {
            get { return Sender.ApproachingBufferCapacity; }
        }

        public bool Disabled { get; private set; }

        public BufferingEventListener(IConfigurationProvider configurationProvider)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException("configurationProvider");
            }

            this.Disabled = !configurationProvider.HasConfiguration;
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventArgs)
        {
            Sender.SubmitEvent(eventArgs.ToEventData());
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (!this.Disabled)
            {
                EnableEvents(eventSource, EventLevel.LogAlways, (EventKeywords)~0);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Sender.Dispose();
        }
    }
}