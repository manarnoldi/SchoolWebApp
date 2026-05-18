using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.Core.Helpers
{
    public static class ApprovalLockHelper
    {
        // Policy constants — stored under GlobalSetting (Module=General, SettingKey=ApprovalEditLockPolicy).
        public const string POLICY_STRICT = "Strict";                   // Lock the moment anything is submitted
        public const string POLICY_AFTER_FIRST_APPROVAL = "AfterFirstApproval"; // Allow edits until the first step is approved

        /// <summary>
        /// Returns true if the given entity is currently locked by an in-flight or approved workflow
        /// and therefore cannot be edited or deleted.
        /// </summary>
        public static async Task<bool> IsLockedAsync(IUnitOfWork uow, string entityType, int entityId)
        {
            var requests = await uow.Repository<ApprovalRequest>().Find(r =>
                r.EntityType == entityType && r.EntityId == entityId,
                includeProperties: "Actions");
            var req = requests.FirstOrDefault();
            if (req == null) return false;

            // Fully approved → always locked (reversal only via Super Admin).
            if (req.Status == ApprovalRequestStatus.Approved) return true;

            // Anything except Submitted (Draft / Rejected / Returned / Reversed) → not locked, user can edit/delete.
            if (req.Status != ApprovalRequestStatus.Submitted) return false;

            // Submitted — decide based on the global policy.
            var settings = await uow.Repository<GlobalSetting>().Find(s =>
                s.Module == "General" && s.SettingKey == "ApprovalEditLockPolicy");
            var policy = settings.FirstOrDefault()?.SettingValue ?? POLICY_STRICT;

            if (policy == POLICY_AFTER_FIRST_APPROVAL)
            {
                // Lenient: editable until the first step is approved.
                return req.Actions.Any(a => a.Status == StepActionStatus.Approved);
            }

            // Strict (default): submitted requests are locked immediately.
            return true;
        }

        /// <summary>
        /// Returns true if the given user is an approver on an ACTIVE workflow (Submitted/Approved)
        /// for this entity. Approvers cannot edit or delete while the workflow is in flight or approved —
        /// but once it's been Rejected / Returned / Reversed, past assignments no longer block.
        /// </summary>
        public static async Task<bool> IsApproverAsync(IUnitOfWork uow, string entityType, int entityId, int userId)
        {
            if (userId <= 0) return false;
            var requests = await uow.Repository<ApprovalRequest>().Find(r =>
                r.EntityType == entityType && r.EntityId == entityId,
                includeProperties: "Actions");
            var req = requests.FirstOrDefault();
            if (req == null) return false;
            if (req.Status != ApprovalRequestStatus.Submitted && req.Status != ApprovalRequestStatus.Approved) return false;
            return req.Actions.Any(a => a.AssignedToUserId == userId);
        }
    }
}
