using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;
using IG.Num;

namespace IG.Gr3d
{

    /// <summary>Interface for 3D graphics controls that can be manipulated via a 
    /// standard set of commands for rotation etc.</summary>
    /// $A Igor xx;
    public interface I3dGraphicsControl
    {

        double RotationStep
        { get; set; }

        /// <summary>Zoom step, in degrees, that is used in a single zoom operation.
        /// <para>Must be greater than 1.</para></summary>
        double ZoomFactor
        { get; set; }

        /// <summary>Changes <see cref="CameraViewAngle"/> by the factor defined by the specified factor.</summary>
        /// <param name="factor">Factor by which the view angle is increased (must be greater than 1).</param>
        void ChangeZoom(double factor);

        /// <summary>Rotates the camera in the fi direction.</summary>
        /// <param name="angleStepDegrees">Step, in degrees, by which the camera is rotated clockwise.</param>
        void RotateAzimuth(double angleStepDegrees);


        /// <summary>Rotates the camera in the theta direction.</summary>
        /// <param name="angleStepDegrees">Step, in degrees, by which the camera is rotated.</param>
        void RotatePitch(double angleStepDegrees);

        /// <summary>Rotates the camera around the viewing direction.</summary>
        /// <param name="angleStepDegrees">Step, in degrees, by which the camera is rotated.</param>
        void RotateRoll(double angleStepDegrees);

        /// <summary>Viewing angle of the camera, in degrees (defines the zoom level).</summary>
        /// <param name="angleStepDegrees">Step, in degrees, by which the camera is rotated.</param>
        double CameraViewAngle
        { get; set; }

        /// <summary>Roll of the camera (amount of rotation abount viewing direction)</summary>
        double CameraRoll
        { get; set; }

        /// <summary>Gets or sets camera position.</summary>
        vec3 CameraPosition
        { get; set; }

        /// <summary>Gets or sets camera focal point.</summary>
        vec3 CameraFocalPoint
        { get; set; }

        /// <summary>Gets or sets the camera viewing up position.</summary>
        vec3 CameraViewUp
        { get; set; }

        /// <summary>Camera direction.
        /// <para>Getter obtains it as difference between the camera focal point and camera position.</para>
        /// <para>Setter sets the camera focal points in such a way that camera direction has the specified value.</para></summary>
        vec3 CameraDirection
        { get; set; }

        /// <summary>Gets or sets camera direction in spherical coordinates.</summary>
        vec3 CameraDirectionSpherical
        { get; set; }

    }  // interface I3dGraphicsControl


}
