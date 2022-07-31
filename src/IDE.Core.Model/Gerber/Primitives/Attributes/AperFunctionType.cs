namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public enum AperFunctionType
{
    //Drill and rout layers
    ViaDrill,
    BackDrill,
    ComponentDrill,
    MechanicalDrill,
    CastellatedDrill,

    //Copper layers

    /// <summary>
    /// A pad associated with a component hole. For THT pads
    /// </summary>
    ComponentPad,

    //SMDPad,(CuDef|SMDef)
    /// <summary>
    /// Confusing spec, don't use !!!
    /// </summary>
    SMDPad,

    //BGAPad,(CuDef|SMDef)
    /// <summary>
    /// Confusing spec, don't use !!!
    /// </summary>
    BGAPad,

    /// <summary>
    /// An edge connector pad
    /// </summary>
    ConnectorPad,

    /// <summary>
    /// Heatsink or thermal pad, typically for SMDs
    /// </summary>
    HeatsinkPad,

    /// <summary>
    /// A via pad
    /// </summary>
    ViaPad,

    TestPad,

    /// <summary>
    /// Pads on plated holes cut-through by the board edge
    /// </summary>
    CastellatedPad,

    //FiducialPad,(Local|Global|Panel)
    FiducialPad,

    /// <summary>
    /// A thermal relief pad connected to the surrounding copper while restricting heat flow
    /// </summary>
    ThermalReliefPad,

    /// <summary>
    /// A pad around a non-plated hole without electrical function. Several applications,
    /// e.g. a pad that strengthens the PCB where fixed with a bolt – hence the name washer.
    /// </summary>
    WasherPad,

    /// <summary>
    /// A pad with clearing polarity (LPC) creating a clearance in a plane.
    /// It makes room for a drill pass without connecting to the plane.
    /// </summary>
    AntiPad,

    /// <summary>
    /// Copper whose function is to connect pads or to provide shielding,
    /// typically tracks and copper pours such as power and ground planes.
    /// Conductive copper pours must carry this attribute.
    /// </summary>
    Conductor,

    /// <summary>
    /// Etched components are embedded inductors, transformers and capacitors which are etched into the PCB copper.
    /// </summary>
    EtchedComponent,

    /// <summary>
    /// Copper that does not serve as a conductor, that has no electrical function; typically text in the PCB such as a part number and version.
    /// Only for copper layers
    /// </summary>
    NonConductor,

    //Component layers

    /// <summary>
    /// This aperture is flashed at the centroid of a component.
    /// The flash carries the object attributes with the main characteristics of the component.
    /// The following aperture must be used:
    /// %ADDnnC,0.300*% (mm)
    /// %ADDnnC,0.012*% (in)
    /// </summary>
    ComponentMain,

    //ComponentOutline,(Body|Lead2Lead| Footprint|Courtyard)
    /// <summary>
    /// This attribute is used to draw the outline of the component.
    /// The following aperture must be used:
    /// %ADDnnC,0.100*% (mm)
    /// %ADDnnC,0.004*% (in)
    /// </summary>
    ComponentOutline,

    /// <summary>
    /// The coordinates in the flash command (D03) indicate the location of the component pins (leads). 
    /// The .P object attribute must be attached to each flash to identify the reference descriptor and pin.
    /// 
    /// For the key pin, typically pin "1" or "A1", the following diamond shape aperture must be used:
    /// %ADDnnP,0.360X4X0.0*% (mm)
    /// %ADDnnP,0.017X4X0.0*% (in)
    /// 
    /// For all other pins the following zero size aperture must be used:
    /// %ADDnnC,0*%...(both mm and in)
    /// </summary>
    ComponentPin,

    //All data layers

    /// <summary>
    /// Identifies the draws and arcs that exactly define the profile or outline of the PCB.
    /// This is the content of the Profile file but can also be present in other layers
    /// </summary>
    Profile,

    /// <summary>
    /// The attribute value NonMaterial identifies objects that do not represent physical material but drawing elements.
    /// NonMaterial is only meaningful on files that define the pattern of physical layers of a PCB such as copper layers or solder mask. 
    /// Such files unfortunately sometimes not only contain data representing material but also drawing elements such as a frame and a title block.
    /// </summary>
    NonMaterial,

    /// <summary>
    /// Identifies the proper part of the data file.
    /// Can be used for solder mask apertures.
    /// For copper and drill layers, use the specific functions (SMDPad) when available rather than ‘Material’
    /// </summary>
    Material
}

