﻿using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Publishing;
using Sitecore.Publishing.Pipelines.PublishItem;
using StaleContent.Constants;
using StaleContent.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaleContent.Pipelines.PublishItem
{
    public class SetRefreshedDates : PublishItemProcessor
    {
        public override void Process(PublishItemContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            if (!SuccessfulPublishState(context))
                return;

            Item sourceItem = context.PublishHelper.GetSourceItem(context.ItemId);
            if (sourceItem.Fields[StatisticsFields.Refreshed] == null)
                return;

            Item targetItem = context.PublishHelper.GetTargetItem(context.ItemId);
            if (targetItem.Fields[StatisticsFields.Refreshed] == null)
                return;

            DateTime now = DateTime.Now;
            FreshnessUtil.Refresh(sourceItem, now);
            FreshnessUtil.Refresh(targetItem, now);
        }


        private bool SuccessfulPublishState(PublishItemContext context)
        {
            if (context == null || context.Result == null)
                return false;

            if (context.Result.Operation != PublishOperation.Created && 
                context.Result.Operation != PublishOperation.Updated)
                return false;

            if (context.Action != PublishAction.PublishVersion)
                return false;

            if (context.VersionToPublish == null)
                return false;

            return true;
        }
    }
}