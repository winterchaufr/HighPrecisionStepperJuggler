﻿using ik = HighPrecisionStepperJuggler.InverseKinematics;

namespace HighPrecisionStepperJuggler
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Translates a High Level Instruction into a Low Level Instruction
        /// </summary>
        public static LLInstruction Translate(this HLInstruction hlInstruction)
        {
            var hlState = hlInstruction.TargetHLMachineState;

            return new LLInstruction(hlState.Translate(), hlInstruction.MoveTime);
        }

        /// <summary>
        /// Translates a High Level Machine State into a Low Level Machine State
        /// </summary>
        public static LLMachineState Translate(this HLMachineState hlState)
        {
            // NOTE: We are adding the origin height because for all IK calculations we need the height relative to
            //       motor shaft position.
            var ikHeight = hlState.Height + Constants.HeightOrigin; 
            
            var xHeightOffset = MiscMath.HeightDifferenceFromTilt(hlState.XTilt);
            var yHeightOffset = MiscMath.HeightDifferenceFromTilt(hlState.YTilt);

            var xWidthOffset = MiscMath.WidthDifferenceFromTilt(hlState.XTilt);
            var yWidthOffset = MiscMath.WidthDifferenceFromTilt(hlState.YTilt);

            var m1Rot = ik.CalculateJoint1RotationFromTargetY(ikHeight + xHeightOffset, xWidthOffset);
            var m2Rot = ik.CalculateJoint1RotationFromTargetY(ikHeight - xHeightOffset, xWidthOffset);
            var m3Rot = ik.CalculateJoint1RotationFromTargetY(ikHeight + yHeightOffset, yWidthOffset);
            var m4Rot = ik.CalculateJoint1RotationFromTargetY(ikHeight - yHeightOffset, yWidthOffset);

            var m1J2Rot = ik.CalculateJoint2RotationFromJoint1Rotation(m1Rot, xWidthOffset);
            var m2J2Rot = ik.CalculateJoint2RotationFromJoint1Rotation(m2Rot, xWidthOffset);
            var m3J2Rot = ik.CalculateJoint2RotationFromJoint1Rotation(m3Rot, yWidthOffset);
            var m4J2Rot = ik.CalculateJoint2RotationFromJoint1Rotation(m4Rot, yWidthOffset);

            return new LLMachineState(m1Rot, m2Rot, m3Rot, m4Rot, m1J2Rot, m2J2Rot, m3J2Rot, m4J2Rot);
        }

        
    }
}