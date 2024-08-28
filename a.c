#include <Windows.h>
#include <stdio.h>
char path[MAX_PATH];
char hotkeyName[4];
DWORD WINAPI ThreadFunc(LPVOID lpParam)
{
	char interval[9];
	char clickType[2];
	GetPrivateProfileString("main", "interval", 0, interval, 9, path);
	GetPrivateProfileString("main", "clickType", 0, clickType, 2, path);
	int type = atoi(clickType);
	printf("\nMouseClickTool 尝试载入配置\n\n间隔: %sms\n按键: %s\n快捷键: %s\n\n正在点击...\n", interval, type == 1 ? "右键" : "左键", hotkeyName);
	int delay = atoi(interval);
	int down = MOUSEEVENTF_LEFTDOWN;
	int up = MOUSEEVENTF_LEFTUP;
	if (type == 1)
	{
		down = MOUSEEVENTF_RIGHTDOWN;
		up = MOUSEEVENTF_RIGHTUP;
	}
	INPUT input;
	input.type = INPUT_MOUSE;
	unsigned long size = sizeof(input);
	while (1)
	{
		input.mi.dwFlags = down;
		SendInput(1, &input, size);
		input.mi.dwFlags = up;
		SendInput(1, &input, size);
		if (delay != 0)
		{
			Sleep(delay);
		}
	}
	return 0;
}
void main()
{
	RECT rect = {0, 0, 440, 440 * 0.618};
	MoveWindow(GetConsoleWindow(), rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top, TRUE);
	GetModuleFileName(NULL, path, sizeof(path));
	strcat(path, ".ini");
	HANDLE hConsole = GetStdHandle(STD_INPUT_HANDLE);
	DWORD mode;
	GetConsoleMode(hConsole, &mode);
	SetConsoleMode(hConsole, mode & ~ENABLE_QUICK_EDIT_MODE);
	char hotkey[4];
	GetPrivateProfileString("main", "hotkey", 0, hotkey, 4, path);
	int keyCode = VK_F1 + atoi(hotkey);
	GetKeyNameText(MapVirtualKey(keyCode, MAPVK_VK_TO_VSC) << 16, hotkeyName, sizeof(hotkeyName));
	printf("启动/停止快捷键为: %s, 若不生效，请更换其他按键\n", hotkeyName);
	UnregisterHotKey(NULL, 1);
	RegisterHotKey(NULL, 1, MOD_NOREPEAT, keyCode);
	HANDLE hThread = NULL;
	DWORD threadId;
	MSG msg = {0};
	while (GetMessage(&msg, NULL, 0, 0))
	{
		UINT mMsg = msg.message;
		if (mMsg == WM_CLOSE || mMsg == WM_DESTROY)
		{
			ExitProcess(0);
		}
		else
		{
			BOOL hkDown = mMsg == WM_HOTKEY;
			if (hThread == NULL && hkDown)
			{
				hThread = CreateThread(
					NULL,
					0,
					ThreadFunc,
					NULL,
					0,
					&threadId);
			}
			else if (hkDown)
			{
				TerminateThread(hThread, 0);
				CloseHandle(hThread);
				hThread = NULL;
				printf("\n结束点击\n");
			}
		}
		Sleep(100);
	}
}