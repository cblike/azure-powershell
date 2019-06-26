﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Rest.Azure;
using Microsoft.Azure.Commands.AlertsManagement.OutputModels;
using Microsoft.Azure.Management.AlertsManagement.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.Commands.AlertsManagement
{
    [Cmdlet("Set", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "ActionRule", SupportsShouldProcess = true)]
    [OutputType(typeof(PSActionRule))]
    public class SetAzureActionRule : AlertsManagementBaseCmdlet
    {
        #region Parameter Set Names

        private const string ByInputObjectParameterSet = "ByInputObject";
        private const string ByJsonFormatActionRuleParameterSet = "ByJsonFormatActionRule";
        private const string BySimplifiedFormatSuppressionActionRuleParameterSet = "BySimplifiedFormatSuppressionActionRule";
        private const string BySimplifiedFormatActionGroupActionRuleParameterSet = "BySimplifiedFormatActionGroupActionRule";

        #endregion

        #region Parameters declarations

        /// <summary>
        /// Gets or sets the input object
        /// </summary>
        [Parameter(ParameterSetName = ByInputObjectParameterSet,
                    Mandatory = true,
                    ValueFromPipeline = true,
                    HelpMessage = "The action rule resource")]
        public PSActionRule InputObject { get; set; }

        /// <summary>
        /// Resource Group Name
        /// </summary>
        [Parameter(Mandatory = true,
                ParameterSetName = ByJsonFormatActionRuleParameterSet,
                HelpMessage = "Resource Group Name")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Resource Group Name")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Resource Group Name")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// Action rule name
        /// </summary>
        [Parameter(Mandatory = true,
                ParameterSetName = ByJsonFormatActionRuleParameterSet,
                HelpMessage = "Action rule Name")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Action rule Name")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Action rule Name")]
        [ValidateNotNullOrEmpty]
        [Alias("ResourceId")]
        public string Name { get; set; }

        /// <summary>
        /// Action rule Json
        /// </summary>
        [Parameter(Mandatory = true,
                ParameterSetName = ByJsonFormatActionRuleParameterSet,
                HelpMessage = "Action rule Json format")]
        [ValidateNotNullOrEmpty]
        public string ActionRule { get; set; }

        /// <summary>
        /// Action rule simplified format : Description
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Description of Action Rule")]
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Description of Action Rule")]
        public string Description { get; set; }

        /// <summary>
        /// Action rule simplified format : Status
        /// </summary>
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Status of Action Rule.")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Status of Action Rule.")]
        [ValidateNotNullOrEmpty]
        [PSArgumentCompleter("Enabled", "Disabled")]
        public string Status { get; set; }

        /// <summary>
        /// Action rule simplified format : Scope Type
        /// </summary>
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Scope Type")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Scope Type")]
        [ValidateNotNullOrEmpty]
        [PSArgumentCompleter("Resource", "ResourceGroup")]
        public string ScopeType { get; set; }

        /// <summary>
        /// Action rule simplified format : List of values
        /// </summary>
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Comma separated list of values")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Comma separated list of values")]
        [ValidateNotNullOrEmpty]
        public string ScopeValues { get; set; }

        /// <summary>
        /// Action rule simplified format : Severity Condition
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Equals:Sev0,Sev1")]
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Equals:Sev0,Sev1")]
        public string SeverityCondition { get; set; }

        /// <summary>
        /// Action rule simplified format : Monitor Service Condition
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Equals:Platform,Log Analytics")]
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Equals:Platform,Log Analytics")]
        public string MonitorServiceCondition { get; set; }

        /// <summary>
        /// Action rule simplified format : Condition for Monitor Condition
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. NotEquals:Resolved")]
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. NotEquals:Resolved")]
        public string MonitorCondition { get; set; }

        /// <summary>
        /// Action rule simplified format : Target Resource Type Condition
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Contains:Virtual Machines,Storage Account")]
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Contains:Virtual Machines,Storage Account")]
        public string TargetResourceTypeCondition { get; set; }

        /// <summary>
        /// Action rule simplified format : Rule ID Condition
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Equals:ARM_ID_1,ARM_ID_2")]
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Equals:ARM_ID_1,ARM_ID_2")]
        public string AlertRuleIdCondition { get; set; }

        /// <summary>
        /// Action rule simplified format : Description Condition
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Contains:Test Alert")]
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Contains:Test Alert")]
        public string DescriptionCondition { get; set; }

        /// <summary>
        /// Action rule simplified format : Alert Context Condition
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Contains:smartgroups")]
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Expected format - {<operation>:<comma separated list of values>} For eg. Contains:smartgroups")]
        public string AlertContextCondition { get; set; }

        /// <summary>
        /// Action rule simplified format : Action Rule Type
        /// </summary>
        [Parameter(Mandatory = true,
                ParameterSetName = ByJsonFormatActionRuleParameterSet,
                HelpMessage = "Action rule Type")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Action rule Type")]
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Action rule Type")]
        [ValidateNotNullOrEmpty]
        [PSArgumentCompleter("Suppression", "ActionGroup", "Diagnostics")]
        public string ActionRuleType { get; set; }

        /// <summary>
        /// Action rule simplified format : Suppression Schedule
        /// </summary>
        [Parameter(Mandatory = true,
                ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                HelpMessage = "Specifies the duration when the suppression should be applied.")]
        [PSArgumentCompleter("Always", "Once", "Daily", "Weekly", "Monthly")]
        public string ReccurenceType { get; set; }

        /// <summary>
        /// Action rule simplified format : Suppression Start Time
        /// </summary>
        [Parameter(Mandatory = false,
               ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
               HelpMessage = "Suppression Start Time. Format 12/09/2018 06:00:00\n +" +
                    "Should be mentioned in case of Reccurent Supression Schedule - Once, Daily, Weekly or Monthly.")]
        public string SuppressionStartTime { get; set; }

        // <summary>
        /// Action rule simplified format : Suppression End Time
        /// </summary>
        [Parameter(Mandatory = false,
               ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
               HelpMessage = "Suppression End Time. Format 12/09/2018 06:00:00\n +" +
                    "Should be mentioned in case of Reccurent Supression Schedule - Once, Daily, Weekly or Monthly.")]
        public string SuppressionEndTime { get; set; }

        // <summary>
        /// Action rule simplified format : Reccurent values
        /// </summary>
        [Parameter(Mandatory = false,
                 ParameterSetName = BySimplifiedFormatSuppressionActionRuleParameterSet,
                 HelpMessage = "Reccurent values, if applicable." +
                    "In case of Weekly - 1,3,5 \n" +
                    "In case of Monthly - 16,24,28 \n")]
        public List<int?> ReccurentValues { get; set; }

        /// <summary>
        /// Action rule simplified format : Action Group Id
        /// </summary>
        [Parameter(Mandatory = false,
                ParameterSetName = BySimplifiedFormatActionGroupActionRuleParameterSet,
                HelpMessage = "Action Group Id which is to be notified.")]
        public string ActionGroupId { get; set; }

        #endregion

        protected override void ProcessRecordInternal()
        {
            PSActionRule actionRule = new PSActionRule();
            switch (ParameterSetName)
            {
                case ByJsonFormatActionRuleParameterSet:
                    // TODO: Update the action rule json string
                    // ActionRule = ActionRule.Replace("\\\\\\", "+");
                    // Deserialize according to action rule type
                    ActionRule actionRuleObject = JsonConvert.DeserializeObject<ActionRule>(ActionRule);
                    actionRule = new PSActionRule(this.AlertsManagementClient.ActionRules.CreateUpdateWithHttpMessagesAsync(
                            resourceGroupName: ResourceGroupName,
                            actionRuleName: Name,
                            actionRule: actionRuleObject
                        ).Result.Body);
                    break;

                case BySimplifiedFormatActionGroupActionRuleParameterSet:
                    // Create Action Rule
                    ActionRule actionGroupAR = new ActionRule(
                        location: "Global",
                        tags: new Dictionary<string, string>(),
                        properties: new ActionGroup(
                            scope: scope,
                            conditions: conditions,
                            actionGroupId: ActionGroupId,
                            description: Description,
                            status: Status
                        )
                    );

                    actionRule = new PSActionRule(this.AlertsManagementClient.ActionRules.CreateUpdateWithHttpMessagesAsync(
                        resourceGroupName: ResourceGroupName, actionRuleName: Name, actionRule: actionGroupAR).Result.Body);
                    break;

                case BySimplifiedFormatSuppressionActionRuleParameterSet:

                    SuppressionConfig config = new SuppressionConfig(recurrenceType: ReccurenceType);
                    if (ReccurenceType != "Daily")
                    {
                        config.Schedule = new SuppressionSchedule(
                            startDate: SuppressionStartTime.Split(' ')[0],
                            endDate: SuppressionEndTime.Split(' ')[0],
                            startTime: SuppressionStartTime.Split(' ')[1],
                            endTime: SuppressionEndTime.Split(' ')[1]
                            );

                        if (ReccurentValues.Count > 0)
                        {
                            config.Schedule.RecurrenceValues = ReccurentValues;
                        }
                    }

                    // Create Action Rule
                    ActionRule suppressionAR = new ActionRule(
                        location: "Global",
                        tags: new Dictionary<string, string>(),
                        properties: new Suppression(
                            scope: scope,
                            conditions: conditions,
                            description: Description,
                            status: Status,
                            suppressionConfig: config
                        )
                    );

                    actionRule = new PSActionRule(this.AlertsManagementClient.ActionRules.CreateUpdateWithHttpMessagesAsync(
                        resourceGroupName: ResourceGroupName, actionRuleName: Name, actionRule: suppressionAR).Result.Body);
                    break;

                case ByInputObjectParameterSet:
                    string[] tokens = InputObject.Id.Split('/');
                    updatedActionRule = new PSActionRule(this.AlertsManagementClient.ActionRules.UpdateWithHttpMessagesAsync(
                        resourceGroupName: tokens[4],
                        actionRuleName: tokens[8],
                        actionRulePatch: new PatchObject(
                                status: Status,
                                tags: Tags
                            )
                        ).Result.Body);
                    //var alert = this.AlertsManagementClient.ActionRules.(AlertId).Result;
                    break;
            }

            WriteObject(sendToPipeline: actionRule);
        }

        private Conditions ParseConditons()
        {
            Conditions conditions = new Conditions();
            if (SeverityCondition != null)
            {
                conditions.Severity = new Condition(
                        operatorProperty: SeverityCondition.Split(':')[0],
                        values: SeverityCondition.Split(':')[1].Split(','));
            }

            if (MonitorServiceCondition != null)
            {
                conditions.MonitorService = new Condition(
                        operatorProperty: MonitorServiceCondition.Split(':')[0],
                        values: MonitorServiceCondition.Split(':')[1].Split(','));
            }

            if (MonitorCondition != null)
            {
                conditions.MonitorCondition = new Condition(
                        operatorProperty: MonitorCondition.Split(':')[0],
                        values: MonitorCondition.Split(':')[1].Split(','));
            }

            if (TargetResourceTypeCondition != null)
            {
                conditions.MonitorCondition = new Condition(
                        operatorProperty: TargetResourceTypeCondition.Split(':')[0],
                        values: TargetResourceTypeCondition.Split(':')[1].Split(','));
            }

            if (DescriptionCondition != null)
            {
                conditions.Description = new Condition(
                        operatorProperty: DescriptionCondition.Split(':')[0],
                        values: DescriptionCondition.Split(':')[1].Split(','));
            }

            if (AlertRuleIdCondition != null)
            {
                conditions.AlertRuleId = new Condition(
                        operatorProperty: AlertRuleIdCondition.Split(':')[0],
                        values: AlertRuleIdCondition.Split(':')[1].Split(','));
            }

            if (AlertContextCondition != null)
            {
                conditions.AlertContext = new Condition(
                        operatorProperty: AlertContextCondition.Split(':')[0],
                        values: AlertContextCondition.Split(':')[1].Split(','));
            }

            return conditions;
        }
    }
}