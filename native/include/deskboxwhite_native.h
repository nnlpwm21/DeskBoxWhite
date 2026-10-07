#ifndef DESKBOXWHITE_NATIVE_H
#define DESKBOXWHITE_NATIVE_H

#include <stdint.h>

#define DESKBOXWHITE_NATIVE_ABI_VERSION 2u
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_STRUCT_VERSION_1 1u
#define DESKBOXWHITE_QUICK_ACCESS_STRUCT_VERSION_1 1u
#define DESKBOXWHITE_RECYCLE_BIN_STRUCT_VERSION_1 1u
#define DESKBOXWHITE_MUSIC_VOLUME_STRUCT_VERSION_1 1u
#define DESKBOXWHITE_NATIVE_STRUCT_VERSION_2 2u
#define DESKBOXWHITE_NATIVE_DLL_NAME L"deskboxwhite_native.dll"

#define DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_READ_STORED_RAW_V2 (1ull << 0)
#define DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_READ_EFFECTIVE_DIAGNOSTIC_V2 (1ull << 1)
#define DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_NO_UI_V2 (1ull << 2)
#define DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_WRITE_V2 (1ull << 3)
#define DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_WITH_UI_V2 (1ull << 4)
#define DESKBOXWHITE_NATIVE_CAPABILITY_MUSIC_VOLUME_V1 (1ull << 5)
#define DESKBOXWHITE_NATIVE_CAPABILITY_EXPLORER_SHELL_LAUNCH_V1 (1ull << 6)
#define DESKBOXWHITE_NATIVE_CAPABILITY_QUICK_ACCESS_V1 (1ull << 7)
#define DESKBOXWHITE_NATIVE_CAPABILITY_RECYCLE_BIN_V1 (1ull << 8)
#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_3C2 \
    (DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_READ_STORED_RAW_V2 | \
     DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_READ_EFFECTIVE_DIAGNOSTIC_V2 | \
     DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_NO_UI_V2 | \
     DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_WRITE_V2 | \
     DESKBOXWHITE_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_WITH_UI_V2)
#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4C \
    (DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_3C2 | \
     DESKBOXWHITE_NATIVE_CAPABILITY_MUSIC_VOLUME_V1)
#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4D4A \
    (DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4C | \
     DESKBOXWHITE_NATIVE_CAPABILITY_EXPLORER_SHELL_LAUNCH_V1)
#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4D4B \
    (DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4D4A | \
     DESKBOXWHITE_NATIVE_CAPABILITY_QUICK_ACCESS_V1)
#define DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_5B4C1B1 \
    (DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_4D4B | \
     DESKBOXWHITE_NATIVE_CAPABILITY_RECYCLE_BIN_V1)
#define DESKBOXWHITE_NATIVE_CAPABILITIES DESKBOXWHITE_NATIVE_CAPABILITIES_STAGE_5B4C1B1

#define DESKBOXWHITE_NATIVE_STATUS_OK 0u
#define DESKBOXWHITE_NATIVE_STATUS_INVALID_ARGUMENT 1u
#define DESKBOXWHITE_NATIVE_STATUS_INCOMPATIBLE_STRUCT 2u
#define DESKBOXWHITE_NATIVE_STATUS_BUFFER_TOO_SMALL 3u
#define DESKBOXWHITE_NATIVE_STATUS_COM_INITIALIZATION_FAILED 4u
#define DESKBOXWHITE_NATIVE_STATUS_OBJECT_CREATION_FAILED 5u
#define DESKBOXWHITE_NATIVE_STATUS_LOAD_FAILED 6u
#define DESKBOXWHITE_NATIVE_STATUS_OPERATION_FAILED 7u
#define DESKBOXWHITE_NATIVE_STATUS_INTERNAL_ERROR 8u
#define DESKBOXWHITE_NATIVE_STATUS_NOT_IMPLEMENTED 9u

#define DESKBOXWHITE_NATIVE_S_OK ((int32_t)0x00000000u)
#define DESKBOXWHITE_NATIVE_S_FALSE ((int32_t)0x00000001u)
#define DESKBOXWHITE_NATIVE_E_NOTIMPL ((int32_t)0x80004001u)
#define DESKBOXWHITE_NATIVE_E_INVALIDARG ((int32_t)0x80070057u)
#define DESKBOXWHITE_NATIVE_HRESULT_INSUFFICIENT_BUFFER ((int32_t)0x8007007Au)
#define DESKBOXWHITE_NATIVE_HRESULT_NOT_ATTEMPTED ((int32_t)0x8000000Au)

#define DESKBOXWHITE_SHORTCUT_READ_MODE_STORED_RAW 1u
#define DESKBOXWHITE_SHORTCUT_READ_MODE_EFFECTIVE_DIAGNOSTIC 2u
#define DESKBOXWHITE_SHORTCUT_WRITE_FLAG_SHELL_NAMESPACE_TARGET (1u << 0)

#define DESKBOXWHITE_SHORTCUT_FIELD_TARGET_PATH (1u << 0)
#define DESKBOXWHITE_SHORTCUT_FIELD_DESCRIPTION (1u << 1)
#define DESKBOXWHITE_SHORTCUT_FIELD_ARGUMENTS (1u << 2)
#define DESKBOXWHITE_SHORTCUT_FIELD_WORKING_DIRECTORY (1u << 3)
#define DESKBOXWHITE_SHORTCUT_FIELD_ICON_PATH (1u << 4)

#define DESKBOXWHITE_SHORTCUT_PHASE_COM_INITIALIZE (1u << 0)
#define DESKBOXWHITE_SHORTCUT_PHASE_CREATE_OBJECT (1u << 1)
#define DESKBOXWHITE_SHORTCUT_PHASE_LOAD (1u << 2)
#define DESKBOXWHITE_SHORTCUT_PHASE_RESOLVE (1u << 3)
#define DESKBOXWHITE_SHORTCUT_PHASE_SAVE (1u << 4)

#define DESKBOXWHITE_MUSIC_VOLUME_OPERATION_GET_SNAPSHOT 1u
#define DESKBOXWHITE_MUSIC_VOLUME_OPERATION_GET_SYSTEM 2u
#define DESKBOXWHITE_MUSIC_VOLUME_OPERATION_SET_SYSTEM 3u
#define DESKBOXWHITE_MUSIC_VOLUME_OPERATION_SET_SESSION 4u

#define DESKBOXWHITE_MUSIC_VOLUME_PHASE_COM_INITIALIZE (1u << 0)
#define DESKBOXWHITE_MUSIC_VOLUME_PHASE_CREATE_ENUMERATOR (1u << 1)
#define DESKBOXWHITE_MUSIC_VOLUME_PHASE_GET_DEVICE (1u << 2)
#define DESKBOXWHITE_MUSIC_VOLUME_PHASE_SYSTEM_VOLUME (1u << 3)
#define DESKBOXWHITE_MUSIC_VOLUME_PHASE_ENUMERATE_SESSIONS (1u << 4)
#define DESKBOXWHITE_MUSIC_VOLUME_PHASE_SESSION_VOLUME (1u << 5)

#define DESKBOXWHITE_MUSIC_VOLUME_MATCH_NONE 0u
#define DESKBOXWHITE_MUSIC_VOLUME_MATCH_IDENTIFIER_APP_ID 1u
#define DESKBOXWHITE_MUSIC_VOLUME_MATCH_INSTANCE_APP_ID 2u
#define DESKBOXWHITE_MUSIC_VOLUME_MATCH_DISPLAY_NAME 3u
#define DESKBOXWHITE_MUSIC_VOLUME_MATCH_PROCESS_DISPLAY_NAME 4u
#define DESKBOXWHITE_MUSIC_VOLUME_MATCH_APP_ID_PROCESS 5u
#define DESKBOXWHITE_MUSIC_VOLUME_MATCH_IDENTIFIER_DISPLAY_NAME 6u
#define DESKBOXWHITE_MUSIC_VOLUME_MATCH_SINGLE_FALLBACK 7u

#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_PHASE_COM_INITIALIZE (1u << 0)
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_PHASE_CREATE_OBJECT (1u << 1)
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_PHASE_WINDOWS (1u << 2)
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_PHASE_DESKTOP (1u << 3)
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_PHASE_DOCUMENT (1u << 4)
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_PHASE_APPLICATION (1u << 5)
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_PHASE_EXECUTE (1u << 6)

#define DESKBOXWHITE_QUICK_ACCESS_OPERATION_QUERY_PIN_STATE 1u
#define DESKBOXWHITE_QUICK_ACCESS_OPERATION_PIN 2u
#define DESKBOXWHITE_QUICK_ACCESS_OPERATION_UNPIN 3u

#define DESKBOXWHITE_QUICK_ACCESS_PIN_STATE_UNKNOWN 0u
#define DESKBOXWHITE_QUICK_ACCESS_PIN_STATE_NOT_PINNED 1u
#define DESKBOXWHITE_QUICK_ACCESS_PIN_STATE_PINNED 2u

#define DESKBOXWHITE_QUICK_ACCESS_PHASE_COM_INITIALIZE (1u << 0)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_CREATE_OBJECT (1u << 1)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_QUICK_NAMESPACE (1u << 2)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_ITEMS (1u << 3)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_ENUMERATE (1u << 4)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_ITEM_PATH (1u << 5)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_PROPERTY (1u << 6)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_PARENT_NAMESPACE (1u << 7)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_PARSE_NAME (1u << 8)
#define DESKBOXWHITE_QUICK_ACCESS_PHASE_INVOKE (1u << 9)

#define DESKBOXWHITE_RECYCLE_BIN_OPERATION_QUERY 1u
#define DESKBOXWHITE_RECYCLE_BIN_OPERATION_RESTORE 2u

#define DESKBOXWHITE_RECYCLE_BIN_PHASE_COM_INITIALIZE (1u << 0)
#define DESKBOXWHITE_RECYCLE_BIN_PHASE_CREATE_OBJECT (1u << 1)
#define DESKBOXWHITE_RECYCLE_BIN_PHASE_NAMESPACE (1u << 2)
#define DESKBOXWHITE_RECYCLE_BIN_PHASE_ITEMS (1u << 3)
#define DESKBOXWHITE_RECYCLE_BIN_PHASE_ENUMERATE (1u << 4)
#define DESKBOXWHITE_RECYCLE_BIN_PHASE_ITEM_NAME (1u << 5)
#define DESKBOXWHITE_RECYCLE_BIN_PHASE_PROPERTY (1u << 6)
#define DESKBOXWHITE_RECYCLE_BIN_PHASE_INVOKE (1u << 7)

#define DESKBOXWHITE_SHORTCUT_DEFAULT_RESOLVE_TIMEOUT_MS 3000u
#define DESKBOXWHITE_SHORTCUT_MAX_RESOLVE_TIMEOUT_MS 65535u
#define DESKBOXWHITE_SHORTCUT_MAX_INPUT_PATH_CHARS 32767u
#define DESKBOXWHITE_SHORTCUT_MAX_INPUT_VALUE_CHARS 32767u
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_MAX_INPUT_CHARS 32767u
#define DESKBOXWHITE_QUICK_ACCESS_MAX_INPUT_CHARS 32767u
#define DESKBOXWHITE_RECYCLE_BIN_MAX_INPUT_CHARS 32767u

#define DESKBOXWHITE_NATIVE_UTF16_BUFFER_V1_SIZE_64 16u
#define DESKBOXWHITE_NATIVE_UTF16_STRING_V1_SIZE_64 16u
#define DESKBOXWHITE_SHORTCUT_READ_REQUEST_V2_SIZE_64 144u
#define DESKBOXWHITE_SHORTCUT_READ_RESULT_V2_SIZE_64 136u
#define DESKBOXWHITE_SHORTCUT_RESOLVE_REQUEST_V2_SIZE_64 192u
#define DESKBOXWHITE_SHORTCUT_WRITE_REQUEST_V2_SIZE_64 144u
#define DESKBOXWHITE_SHORTCUT_WRITE_RESULT_V2_SIZE_64 96u
#define DESKBOXWHITE_SHORTCUT_UI_RESOLVE_REQUEST_V2_SIZE_64 64u
#define DESKBOXWHITE_SHORTCUT_UI_RESOLVE_RESULT_V2_SIZE_64 64u
#define DESKBOXWHITE_MUSIC_VOLUME_REQUEST_V1_SIZE_64 88u
#define DESKBOXWHITE_MUSIC_VOLUME_RESULT_V1_SIZE_64 104u
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_REQUEST_V1_SIZE_64 96u
#define DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_RESULT_V1_SIZE_64 88u
#define DESKBOXWHITE_QUICK_ACCESS_REQUEST_V1_SIZE_64 96u
#define DESKBOXWHITE_QUICK_ACCESS_RESULT_V1_SIZE_64 112u
#define DESKBOXWHITE_RECYCLE_BIN_REQUEST_V1_SIZE_64 80u
#define DESKBOXWHITE_RECYCLE_BIN_RESULT_V1_SIZE_64 104u

#if defined(_WIN32)
#define DESKBOXWHITE_NATIVE_API __declspec(dllimport)
#define DESKBOXWHITE_NATIVE_CALL __cdecl
#else
#define DESKBOXWHITE_NATIVE_API
#define DESKBOXWHITE_NATIVE_CALL
#endif

typedef struct DeskBoxWhiteNativeUtf16BufferV1 {
    uint16_t* data;
    uint32_t capacity_chars;
    uint32_t reserved0;
} DeskBoxWhiteNativeUtf16BufferV1;

typedef struct DeskBoxWhiteNativeUtf16StringV1 {
    const uint16_t* data;
    uint32_t length_chars;
    uint32_t reserved0;
} DeskBoxWhiteNativeUtf16StringV1;

typedef struct DeskBoxWhiteShortcutReadRequestV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t mode;
    uint32_t flags;
    const uint16_t* shortcut_path;
    uint32_t shortcut_path_length_chars;
    uint32_t reserved0;
    DeskBoxWhiteNativeUtf16BufferV1 target_path;
    DeskBoxWhiteNativeUtf16BufferV1 description;
    DeskBoxWhiteNativeUtf16BufferV1 arguments;
    DeskBoxWhiteNativeUtf16BufferV1 working_directory;
    DeskBoxWhiteNativeUtf16BufferV1 icon_path;
    uint64_t reserved[4];
} DeskBoxWhiteShortcutReadRequestV2;

typedef struct DeskBoxWhiteShortcutReadResultV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t load_hresult;
    int32_t resolve_hresult;
    uint32_t attempted_fields;
    uint32_t succeeded_fields;
    uint32_t present_fields;
    uint32_t caller_buffer_too_small_fields;
    uint32_t source_truncated_fields;
    int32_t target_hresult;
    int32_t description_hresult;
    int32_t arguments_hresult;
    int32_t working_directory_hresult;
    int32_t icon_hresult;
    int32_t icon_index;
    uint32_t target_required_chars;
    uint32_t description_required_chars;
    uint32_t arguments_required_chars;
    uint32_t working_directory_required_chars;
    uint32_t icon_required_chars;
    uint32_t reserved0;
    uint64_t reserved[4];
} DeskBoxWhiteShortcutReadResultV2;

typedef struct DeskBoxWhiteShortcutResolveRequestV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t timeout_ms;
    uint32_t flags;
    DeskBoxWhiteShortcutReadRequestV2 read_request;
    uint64_t reserved[4];
} DeskBoxWhiteShortcutResolveRequestV2;

typedef struct DeskBoxWhiteShortcutWriteRequestV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t flags;
    int32_t icon_index;
    DeskBoxWhiteNativeUtf16StringV1 shortcut_path;
    DeskBoxWhiteNativeUtf16StringV1 target_path;
    DeskBoxWhiteNativeUtf16StringV1 description;
    DeskBoxWhiteNativeUtf16StringV1 arguments;
    DeskBoxWhiteNativeUtf16StringV1 working_directory;
    DeskBoxWhiteNativeUtf16StringV1 icon_path;
    uint64_t reserved[4];
} DeskBoxWhiteShortcutWriteRequestV2;

typedef struct DeskBoxWhiteShortcutWriteResultV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t save_hresult;
    uint32_t attempted_fields;
    uint32_t succeeded_fields;
    int32_t target_hresult;
    int32_t description_hresult;
    int32_t arguments_hresult;
    int32_t working_directory_hresult;
    int32_t icon_hresult;
    uint32_t reserved0;
    uint64_t reserved[4];
} DeskBoxWhiteShortcutWriteResultV2;

typedef struct DeskBoxWhiteShortcutUiResolveRequestV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t flags;
    uint32_t reserved0;
    DeskBoxWhiteNativeUtf16StringV1 shortcut_path;
    uint64_t owner_hwnd;
    uint64_t reserved[3];
} DeskBoxWhiteShortcutUiResolveRequestV2;

typedef struct DeskBoxWhiteShortcutUiResolveResultV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t load_hresult;
    int32_t resolve_hresult;
    uint32_t resolve_flags;
    uint64_t reserved[3];
} DeskBoxWhiteShortcutUiResolveResultV2;

typedef struct DeskBoxWhiteMusicVolumeRequestV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t operation;
    uint32_t flags;
    DeskBoxWhiteNativeUtf16StringV1 source_app_user_model_id;
    DeskBoxWhiteNativeUtf16StringV1 source_display_name;
    double volume;
    uint64_t reserved[4];
} DeskBoxWhiteMusicVolumeRequestV1;

typedef struct DeskBoxWhiteMusicVolumeResultV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    uint32_t match_kind;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t device_hresult;
    int32_t system_hresult;
    int32_t session_hresult;
    uint32_t has_session_volume;
    uint32_t operation_succeeded;
    uint32_t reserved0;
    double system_volume;
    double session_volume;
    uint64_t reserved[4];
} DeskBoxWhiteMusicVolumeResultV1;

typedef struct DeskBoxWhiteExplorerShellLaunchRequestV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t flags;
    uint32_t reserved0;
    DeskBoxWhiteNativeUtf16StringV1 path;
    DeskBoxWhiteNativeUtf16StringV1 working_directory;
    DeskBoxWhiteNativeUtf16StringV1 verb;
    uint64_t reserved[4];
} DeskBoxWhiteExplorerShellLaunchRequestV1;

typedef struct DeskBoxWhiteExplorerShellLaunchResultV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t windows_hresult;
    int32_t desktop_hresult;
    int32_t document_hresult;
    int32_t application_hresult;
    int32_t execute_hresult;
    uint32_t operation_succeeded;
    uint32_t reserved0;
    uint64_t reserved[4];
} DeskBoxWhiteExplorerShellLaunchResultV1;

typedef struct DeskBoxWhiteQuickAccessRequestV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t operation;
    uint32_t flags;
    DeskBoxWhiteNativeUtf16StringV1 folder_path;
    DeskBoxWhiteNativeUtf16StringV1 parent_path;
    DeskBoxWhiteNativeUtf16StringV1 folder_name;
    uint64_t reserved[4];
} DeskBoxWhiteQuickAccessRequestV1;

typedef struct DeskBoxWhiteQuickAccessResultV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t quick_namespace_hresult;
    int32_t items_hresult;
    int32_t enumerate_hresult;
    int32_t item_path_hresult;
    int32_t property_hresult;
    int32_t parent_namespace_hresult;
    int32_t parse_name_hresult;
    int32_t invoke_hresult;
    uint32_t pin_state;
    uint32_t operation_succeeded;
    uint32_t matched_item;
    uint32_t fallback_used;
    uint32_t reserved0;
    uint64_t reserved[4];
} DeskBoxWhiteQuickAccessResultV1;

typedef struct DeskBoxWhiteRecycleBinRequestV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t operation;
    uint32_t flags;
    DeskBoxWhiteNativeUtf16StringV1 original_parent;
    DeskBoxWhiteNativeUtf16StringV1 original_name;
    uint64_t reserved[4];
} DeskBoxWhiteRecycleBinRequestV1;

typedef struct DeskBoxWhiteRecycleBinResultV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t namespace_hresult;
    int32_t items_hresult;
    int32_t enumerate_hresult;
    int32_t item_name_hresult;
    int32_t property_hresult;
    int32_t invoke_hresult;
    uint32_t matched_count;
    uint32_t restored_count;
    uint32_t operation_succeeded;
    uint32_t reserved0;
    uint32_t reserved1;
    uint64_t reserved[4];
} DeskBoxWhiteRecycleBinResultV1;

#if UINTPTR_MAX == UINT64_MAX
#if defined(__cplusplus)
static_assert(sizeof(DeskBoxWhiteNativeUtf16BufferV1) == DESKBOXWHITE_NATIVE_UTF16_BUFFER_V1_SIZE_64,
              "DeskBoxWhiteNativeUtf16BufferV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteNativeUtf16StringV1) == DESKBOXWHITE_NATIVE_UTF16_STRING_V1_SIZE_64,
              "DeskBoxWhiteNativeUtf16StringV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteShortcutReadRequestV2) == DESKBOXWHITE_SHORTCUT_READ_REQUEST_V2_SIZE_64,
              "DeskBoxWhiteShortcutReadRequestV2 ABI size changed");
static_assert(sizeof(DeskBoxWhiteShortcutReadResultV2) == DESKBOXWHITE_SHORTCUT_READ_RESULT_V2_SIZE_64,
              "DeskBoxWhiteShortcutReadResultV2 ABI size changed");
static_assert(sizeof(DeskBoxWhiteShortcutResolveRequestV2) == DESKBOXWHITE_SHORTCUT_RESOLVE_REQUEST_V2_SIZE_64,
              "DeskBoxWhiteShortcutResolveRequestV2 ABI size changed");
static_assert(sizeof(DeskBoxWhiteShortcutWriteRequestV2) == DESKBOXWHITE_SHORTCUT_WRITE_REQUEST_V2_SIZE_64,
              "DeskBoxWhiteShortcutWriteRequestV2 ABI size changed");
static_assert(sizeof(DeskBoxWhiteShortcutWriteResultV2) == DESKBOXWHITE_SHORTCUT_WRITE_RESULT_V2_SIZE_64,
              "DeskBoxWhiteShortcutWriteResultV2 ABI size changed");
static_assert(sizeof(DeskBoxWhiteShortcutUiResolveRequestV2) == DESKBOXWHITE_SHORTCUT_UI_RESOLVE_REQUEST_V2_SIZE_64,
              "DeskBoxWhiteShortcutUiResolveRequestV2 ABI size changed");
static_assert(sizeof(DeskBoxWhiteShortcutUiResolveResultV2) == DESKBOXWHITE_SHORTCUT_UI_RESOLVE_RESULT_V2_SIZE_64,
              "DeskBoxWhiteShortcutUiResolveResultV2 ABI size changed");
static_assert(sizeof(DeskBoxWhiteMusicVolumeRequestV1) == DESKBOXWHITE_MUSIC_VOLUME_REQUEST_V1_SIZE_64,
              "DeskBoxWhiteMusicVolumeRequestV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteMusicVolumeResultV1) == DESKBOXWHITE_MUSIC_VOLUME_RESULT_V1_SIZE_64,
              "DeskBoxWhiteMusicVolumeResultV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteExplorerShellLaunchRequestV1) == DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_REQUEST_V1_SIZE_64,
              "DeskBoxWhiteExplorerShellLaunchRequestV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteExplorerShellLaunchResultV1) == DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_RESULT_V1_SIZE_64,
              "DeskBoxWhiteExplorerShellLaunchResultV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteQuickAccessRequestV1) == DESKBOXWHITE_QUICK_ACCESS_REQUEST_V1_SIZE_64,
              "DeskBoxWhiteQuickAccessRequestV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteQuickAccessResultV1) == DESKBOXWHITE_QUICK_ACCESS_RESULT_V1_SIZE_64,
              "DeskBoxWhiteQuickAccessResultV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteRecycleBinRequestV1) == DESKBOXWHITE_RECYCLE_BIN_REQUEST_V1_SIZE_64,
              "DeskBoxWhiteRecycleBinRequestV1 ABI size changed");
static_assert(sizeof(DeskBoxWhiteRecycleBinResultV1) == DESKBOXWHITE_RECYCLE_BIN_RESULT_V1_SIZE_64,
              "DeskBoxWhiteRecycleBinResultV1 ABI size changed");
#elif defined(__STDC_VERSION__) && __STDC_VERSION__ >= 201112L
_Static_assert(sizeof(DeskBoxWhiteNativeUtf16BufferV1) == DESKBOXWHITE_NATIVE_UTF16_BUFFER_V1_SIZE_64,
               "DeskBoxWhiteNativeUtf16BufferV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteNativeUtf16StringV1) == DESKBOXWHITE_NATIVE_UTF16_STRING_V1_SIZE_64,
               "DeskBoxWhiteNativeUtf16StringV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteShortcutReadRequestV2) == DESKBOXWHITE_SHORTCUT_READ_REQUEST_V2_SIZE_64,
               "DeskBoxWhiteShortcutReadRequestV2 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteShortcutReadResultV2) == DESKBOXWHITE_SHORTCUT_READ_RESULT_V2_SIZE_64,
               "DeskBoxWhiteShortcutReadResultV2 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteShortcutResolveRequestV2) == DESKBOXWHITE_SHORTCUT_RESOLVE_REQUEST_V2_SIZE_64,
               "DeskBoxWhiteShortcutResolveRequestV2 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteShortcutWriteRequestV2) == DESKBOXWHITE_SHORTCUT_WRITE_REQUEST_V2_SIZE_64,
               "DeskBoxWhiteShortcutWriteRequestV2 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteShortcutWriteResultV2) == DESKBOXWHITE_SHORTCUT_WRITE_RESULT_V2_SIZE_64,
               "DeskBoxWhiteShortcutWriteResultV2 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteShortcutUiResolveRequestV2) == DESKBOXWHITE_SHORTCUT_UI_RESOLVE_REQUEST_V2_SIZE_64,
               "DeskBoxWhiteShortcutUiResolveRequestV2 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteShortcutUiResolveResultV2) == DESKBOXWHITE_SHORTCUT_UI_RESOLVE_RESULT_V2_SIZE_64,
               "DeskBoxWhiteShortcutUiResolveResultV2 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteMusicVolumeRequestV1) == DESKBOXWHITE_MUSIC_VOLUME_REQUEST_V1_SIZE_64,
               "DeskBoxWhiteMusicVolumeRequestV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteMusicVolumeResultV1) == DESKBOXWHITE_MUSIC_VOLUME_RESULT_V1_SIZE_64,
               "DeskBoxWhiteMusicVolumeResultV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteExplorerShellLaunchRequestV1) == DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_REQUEST_V1_SIZE_64,
               "DeskBoxWhiteExplorerShellLaunchRequestV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteExplorerShellLaunchResultV1) == DESKBOXWHITE_EXPLORER_SHELL_LAUNCH_RESULT_V1_SIZE_64,
               "DeskBoxWhiteExplorerShellLaunchResultV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteQuickAccessRequestV1) == DESKBOXWHITE_QUICK_ACCESS_REQUEST_V1_SIZE_64,
               "DeskBoxWhiteQuickAccessRequestV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteQuickAccessResultV1) == DESKBOXWHITE_QUICK_ACCESS_RESULT_V1_SIZE_64,
               "DeskBoxWhiteQuickAccessResultV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteRecycleBinRequestV1) == DESKBOXWHITE_RECYCLE_BIN_REQUEST_V1_SIZE_64,
               "DeskBoxWhiteRecycleBinRequestV1 ABI size changed");
_Static_assert(sizeof(DeskBoxWhiteRecycleBinResultV1) == DESKBOXWHITE_RECYCLE_BIN_RESULT_V1_SIZE_64,
               "DeskBoxWhiteRecycleBinResultV1 ABI size changed");
#endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_native_abi_version(void);
DESKBOXWHITE_NATIVE_API uint64_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_native_capabilities(void);
DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_shortcut_read_v2(
    const DeskBoxWhiteShortcutReadRequestV2* request,
    DeskBoxWhiteShortcutReadResultV2* result);
DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_shortcut_resolve_no_ui_v2(
    const DeskBoxWhiteShortcutResolveRequestV2* request,
    DeskBoxWhiteShortcutReadResultV2* result);
DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_shortcut_write_v2(
    const DeskBoxWhiteShortcutWriteRequestV2* request,
    DeskBoxWhiteShortcutWriteResultV2* result);
DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_shortcut_resolve_with_ui_v2(
    const DeskBoxWhiteShortcutUiResolveRequestV2* request,
    DeskBoxWhiteShortcutUiResolveResultV2* result);
DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_music_volume_v1(
    const DeskBoxWhiteMusicVolumeRequestV1* request,
    DeskBoxWhiteMusicVolumeResultV1* result);
DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_explorer_shell_launch_v1(
    const DeskBoxWhiteExplorerShellLaunchRequestV1* request,
    DeskBoxWhiteExplorerShellLaunchResultV1* result);
DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_quick_access_v1(
    const DeskBoxWhiteQuickAccessRequestV1* request,
    DeskBoxWhiteQuickAccessResultV1* result);
DESKBOXWHITE_NATIVE_API uint32_t DESKBOXWHITE_NATIVE_CALL deskboxwhite_recycle_bin_v1(
    const DeskBoxWhiteRecycleBinRequestV1* request,
    DeskBoxWhiteRecycleBinResultV1* result);

#ifdef __cplusplus
}
#endif

#endif
