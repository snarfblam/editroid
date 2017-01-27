using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
    /// <summary>
    /// Provides a class to provide recursive begin-update/end-update behavior.
    /// The class can either be inherited or used with an IUpdatable source to
    /// extend behavior.
    /// </summary>
    /// <typeparam name="TSource">The class that will own the update scope.</typeparam>
    class UpdateScoper<TSource> : IActionScoped
    {
        TSource source;
        bool isUpdatable = typeof(IUpdatable).IsAssignableFrom(typeof(TSource));

        public UpdateScoper(TSource source) {
            this.source = source;
        }

        public TSource Source { get { return source; } }

        int updateLevel = 0;

        /// <summary>
        /// Increases the update level.
        /// </summary>
        /// <returns>An ActionScope which, when disposed, will call EndUpdate().</returns>
        public ActionScope BeginUpdate() {
            updateLevel++;

            if (updateLevel == 1) {
                OnUpdateStart();
                if (isUpdatable)
                    ((IUpdatable)source).OnUpdateStart();
            }

            return new SimpleActionScope(this);
        }

        public void EndUpdate() {
            updateLevel--;

            if (updateLevel == 0) {
                OnUpdateEnd();
                if (isUpdatable)
                    ((IUpdatable)source).OnUpdateEnd();
            }
        }

        public bool IsUpdating { get { return updateLevel != 0; } }

        /// <summary>
        /// Called when IsUpdating changes from false to true.
        /// </summary>
        protected virtual void OnUpdateStart() {
        }

        /// <summary>
        /// Called when IsUpdating changes from true to false.
        /// </summary>
        protected virtual void OnUpdateEnd() {
        }
        void IActionScoped.EndAction() {
            EndUpdate();
        }
    }

    /// <summary>
    /// If implemented, the interface members will be called by an
    /// UpdateScoper to inform of changes in update status.
    /// </summary>
    public interface IUpdatable
    {
        void OnUpdateStart();
        void OnUpdateEnd();
    }
}
