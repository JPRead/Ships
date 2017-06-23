using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine7;
namespace Template
{
    /// <summary>
    /// defines a message types
    /// </summary>
    class ExtraMessageTypes : MessageType
    {

        /// <summary>
        /// no message data sent when a player bullet is destroyed
        /// </summary>
        public static readonly MessageType BulletDestroyed = new MessageType("BulletDestroyed");
        /// <summary>
        /// no message data sent when player is destroyed
        /// </summary>
        public static readonly MessageType PlayerDestroyed = new MessageType("PlayerDestroyed");
        /// <summary>
        /// object value contains an integer for score value of enemy
        /// </summary>
        public static readonly MessageType EnemyDestroyed = new MessageType("EnemyDestroyed");
        /// <summary>
        /// when score is finished, initials are stored in Object which needs casting to (string)
        /// </summary>
        public static readonly MessageType NameEntryComplete = new MessageType("NameEntryComplete");
    }
}
