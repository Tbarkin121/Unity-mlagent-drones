// GENERATED AUTOMATICALLY FROM 'Assets/SharedAssets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""08c37f75-a738-4cbe-94b9-cce72b8278b8"",
            ""actions"": [
                {
                    ""name"": ""XYVec"",
                    ""type"": ""Value"",
                    ""id"": ""0fb14bec-2998-4ae2-b3c9-836244484e8c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""YYawVec"",
                    ""type"": ""Value"",
                    ""id"": ""84681808-d1ae-4235-9fab-c79bb69360fc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Speed"",
                    ""type"": ""Value"",
                    ""id"": ""d4074595-17a3-4ef7-b40b-4b0f5559637a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1abe5295-b001-4a10-808f-5d12eca60ccc"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""XYVec"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a3692f2-9460-4477-a75b-505ebb611c1d"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""YYawVec"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7568488c-b948-41f3-8545-cb713842dedf"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Speed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gameplay"",
            ""bindingGroup"": ""Gameplay"",
            ""devices"": []
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_XYVec = m_Gameplay.FindAction("XYVec", throwIfNotFound: true);
        m_Gameplay_YYawVec = m_Gameplay.FindAction("YYawVec", throwIfNotFound: true);
        m_Gameplay_Speed = m_Gameplay.FindAction("Speed", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_XYVec;
    private readonly InputAction m_Gameplay_YYawVec;
    private readonly InputAction m_Gameplay_Speed;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @XYVec => m_Wrapper.m_Gameplay_XYVec;
        public InputAction @YYawVec => m_Wrapper.m_Gameplay_YYawVec;
        public InputAction @Speed => m_Wrapper.m_Gameplay_Speed;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @XYVec.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnXYVec;
                @XYVec.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnXYVec;
                @XYVec.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnXYVec;
                @YYawVec.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnYYawVec;
                @YYawVec.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnYYawVec;
                @YYawVec.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnYYawVec;
                @Speed.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSpeed;
                @Speed.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSpeed;
                @Speed.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSpeed;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @XYVec.started += instance.OnXYVec;
                @XYVec.performed += instance.OnXYVec;
                @XYVec.canceled += instance.OnXYVec;
                @YYawVec.started += instance.OnYYawVec;
                @YYawVec.performed += instance.OnYYawVec;
                @YYawVec.canceled += instance.OnYYawVec;
                @Speed.started += instance.OnSpeed;
                @Speed.performed += instance.OnSpeed;
                @Speed.canceled += instance.OnSpeed;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    private int m_GameplaySchemeIndex = -1;
    public InputControlScheme GameplayScheme
    {
        get
        {
            if (m_GameplaySchemeIndex == -1) m_GameplaySchemeIndex = asset.FindControlSchemeIndex("Gameplay");
            return asset.controlSchemes[m_GameplaySchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnXYVec(InputAction.CallbackContext context);
        void OnYYawVec(InputAction.CallbackContext context);
        void OnSpeed(InputAction.CallbackContext context);
    }
}
