.387
.model flat, stdcall 
.xmm
.data
.code

;-----------------------------------------------------------;
; Funkcja testujaca.
Dodaj proc uses ebx a:DWORD, b:DWORD
	mov eax,a
	mov ebx,b
	add eax, ebx
	ret	
Dodaj endp 

;-----------------------------------------------------------;
GreyASM PROC bitmap : dword, bWidth : dword, bHeight : dword
	pushad
	mov esi, bitmap			; ESI <- adres bitmapy
	mov eax, bWidth
	mov ebx, bHeight
	mul ebx
	mov ebx, 3				
	mul ebx					; EAX <- rozmiar obrazka w bajtach
	mov ecx, eax			; ECX <- rozmiar obrazka w bajtach
	add ecx, esi			; ECX <- adres zakonczenia procedury

	assume esi:ptr byte		; ESI wskazuje na jeden bajt pamieci
	_loop:
			xor eax, eax	; Zerowanie rejestru
			xor ebx, ebx	; Zerowanie rejestru
			xor edx, edx	; Zerowanie rejestru
			mov al, [esi]	; AL <- skladowa R
			mov bl, [esi+1] ; BL <- skladowa G
			add eax, ebx	; EAX <- R+B
			mov bl, [esi+2]	; BL <- skladowa B
			add eax, ebx	; EAX <- R+B+G
			mov ebx, 3		; EBX <- 3 (dzielnik)
			div ebx			; AL <- R+B+G/3
			mov [esi], al	; NowaR <- AL
			mov [esi+1], al ; NowaG <- AL
			mov [esi+2], al ; NowaB <- AL

			add esi, 3
			cmp ecx, esi
			ja _loop
	endloop:
	popad
	ret
GreyASM ENDP
;-----------------------------------------------------------;
end 