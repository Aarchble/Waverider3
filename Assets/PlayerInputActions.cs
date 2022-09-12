//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""FlightControls"",
            ""id"": ""195790f8-b936-45be-be44-3ccac0a67b23"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""2f8b7908-b7aa-450b-9f9a-66bdd5a5b92d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7b9cc17d-1f56-48c6-985c-e036b98569e5"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""BuildingControls"",
            ""id"": ""b813089c-be60-486a-abd3-cdda3ece5983"",
            ""actions"": [
                {
                    ""name"": ""DragAndMove"",
                    ""type"": ""Value"",
                    ""id"": ""d8b1b20f-0b20-4d54-8854-23a838b98315"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""698b1599-76fd-4c14-a601-ef4a7c6a3c9f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DragAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""One Modifier"",
                    ""id"": ""2623bd6c-991a-4a35-93d8-62a11132d4eb"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DragAndMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""56dee124-c38d-4192-8b06-e6f768041294"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DragAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""e2e3fe88-018e-4f13-901b-2e89b937f318"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DragAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""ManagementControls"",
            ""id"": ""06e543aa-7f31-4c09-b13f-fb90fc010d47"",
            ""actions"": [
                {
                    ""name"": ""SwitchBuildFly"",
                    ""type"": ""Button"",
                    ""id"": ""172fcace-ab5d-4cef-adec-8125509092d4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""93ce0b80-1666-427f-946d-2827dc28e841"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchBuildFly"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // FlightControls
        m_FlightControls = asset.FindActionMap("FlightControls", throwIfNotFound: true);
        m_FlightControls_Newaction = m_FlightControls.FindAction("New action", throwIfNotFound: true);
        // BuildingControls
        m_BuildingControls = asset.FindActionMap("BuildingControls", throwIfNotFound: true);
        m_BuildingControls_DragAndMove = m_BuildingControls.FindAction("DragAndMove", throwIfNotFound: true);
        // ManagementControls
        m_ManagementControls = asset.FindActionMap("ManagementControls", throwIfNotFound: true);
        m_ManagementControls_SwitchBuildFly = m_ManagementControls.FindAction("SwitchBuildFly", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // FlightControls
    private readonly InputActionMap m_FlightControls;
    private IFlightControlsActions m_FlightControlsActionsCallbackInterface;
    private readonly InputAction m_FlightControls_Newaction;
    public struct FlightControlsActions
    {
        private @PlayerInputActions m_Wrapper;
        public FlightControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_FlightControls_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_FlightControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FlightControlsActions set) { return set.Get(); }
        public void SetCallbacks(IFlightControlsActions instance)
        {
            if (m_Wrapper.m_FlightControlsActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_FlightControlsActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_FlightControlsActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_FlightControlsActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_FlightControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public FlightControlsActions @FlightControls => new FlightControlsActions(this);

    // BuildingControls
    private readonly InputActionMap m_BuildingControls;
    private IBuildingControlsActions m_BuildingControlsActionsCallbackInterface;
    private readonly InputAction m_BuildingControls_DragAndMove;
    public struct BuildingControlsActions
    {
        private @PlayerInputActions m_Wrapper;
        public BuildingControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @DragAndMove => m_Wrapper.m_BuildingControls_DragAndMove;
        public InputActionMap Get() { return m_Wrapper.m_BuildingControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BuildingControlsActions set) { return set.Get(); }
        public void SetCallbacks(IBuildingControlsActions instance)
        {
            if (m_Wrapper.m_BuildingControlsActionsCallbackInterface != null)
            {
                @DragAndMove.started -= m_Wrapper.m_BuildingControlsActionsCallbackInterface.OnDragAndMove;
                @DragAndMove.performed -= m_Wrapper.m_BuildingControlsActionsCallbackInterface.OnDragAndMove;
                @DragAndMove.canceled -= m_Wrapper.m_BuildingControlsActionsCallbackInterface.OnDragAndMove;
            }
            m_Wrapper.m_BuildingControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DragAndMove.started += instance.OnDragAndMove;
                @DragAndMove.performed += instance.OnDragAndMove;
                @DragAndMove.canceled += instance.OnDragAndMove;
            }
        }
    }
    public BuildingControlsActions @BuildingControls => new BuildingControlsActions(this);

    // ManagementControls
    private readonly InputActionMap m_ManagementControls;
    private IManagementControlsActions m_ManagementControlsActionsCallbackInterface;
    private readonly InputAction m_ManagementControls_SwitchBuildFly;
    public struct ManagementControlsActions
    {
        private @PlayerInputActions m_Wrapper;
        public ManagementControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @SwitchBuildFly => m_Wrapper.m_ManagementControls_SwitchBuildFly;
        public InputActionMap Get() { return m_Wrapper.m_ManagementControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ManagementControlsActions set) { return set.Get(); }
        public void SetCallbacks(IManagementControlsActions instance)
        {
            if (m_Wrapper.m_ManagementControlsActionsCallbackInterface != null)
            {
                @SwitchBuildFly.started -= m_Wrapper.m_ManagementControlsActionsCallbackInterface.OnSwitchBuildFly;
                @SwitchBuildFly.performed -= m_Wrapper.m_ManagementControlsActionsCallbackInterface.OnSwitchBuildFly;
                @SwitchBuildFly.canceled -= m_Wrapper.m_ManagementControlsActionsCallbackInterface.OnSwitchBuildFly;
            }
            m_Wrapper.m_ManagementControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SwitchBuildFly.started += instance.OnSwitchBuildFly;
                @SwitchBuildFly.performed += instance.OnSwitchBuildFly;
                @SwitchBuildFly.canceled += instance.OnSwitchBuildFly;
            }
        }
    }
    public ManagementControlsActions @ManagementControls => new ManagementControlsActions(this);
    public interface IFlightControlsActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
    public interface IBuildingControlsActions
    {
        void OnDragAndMove(InputAction.CallbackContext context);
    }
    public interface IManagementControlsActions
    {
        void OnSwitchBuildFly(InputAction.CallbackContext context);
    }
}
