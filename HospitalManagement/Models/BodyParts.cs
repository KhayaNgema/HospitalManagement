using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum BodyParts
    {
        [Display(Name = "Chest")]
        Chest,

        [Display(Name = "Abdomen")]
        Abdomen,

        [Display(Name = "Skull")]
        Skull,

        [Display(Name = "Cervical Spine")]
        SpineCervical,

        [Display(Name = "Thoracic Spine")]
        SpineThoracic,

        [Display(Name = "Lumbar Spine")]
        SpineLumbar,

        [Display(Name = "Pelvis")]
        Pelvis,

        [Display(Name = "Left Shoulder")]
        ShoulderLeft,

        [Display(Name = "Right Shoulder")]
        ShoulderRight,

        [Display(Name = "Left Arm")]
        ArmLeft,

        [Display(Name = "Right Arm")]
        ArmRight,

        [Display(Name = "Left Elbow")]
        ElbowLeft,

        [Display(Name = "Right Elbow")]
        ElbowRight,

        [Display(Name = "Left Wrist")]
        WristLeft,

        [Display(Name = "Right Wrist")]
        WristRight,

        [Display(Name = "Left Hand")]
        HandLeft,

        [Display(Name = "Right Hand")]
        HandRight,

        [Display(Name = "Left Hip")]
        HipLeft,

        [Display(Name = "Right Hip")]
        HipRight,

        [Display(Name = "Left Knee")]
        KneeLeft,

        [Display(Name = "Right Knee")]
        KneeRight,

        [Display(Name = "Left Ankle")]
        AnkleLeft,

        [Display(Name = "Right Ankle")]
        AnkleRight,

        [Display(Name = "Left Foot")]
        FootLeft,

        [Display(Name = "Right Foot")]
        FootRight,

        [Display(Name = "Ribs")]
        Ribs,

        [Display(Name = "Neck")]
        Neck,

        [Display(Name = "Sinuses")]
        Sinuses
    }
}
