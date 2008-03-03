\ ==============================================================================
\
\            trm - a terminal escape sequences outputter in the ffl
\
\                Copyright (C) 2006  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2007-12-09 07:23:17 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] trm.version [IF]

( trm = Terminal Escape Sequence Outputter )
( The trm module implements an outputter for terminal escape sequences.        )
( It supports a selection of ANSI, VT100, VT102, ECMA-48 and linux console     )
( specific escape sequences in order to perform special terminal actions like  )
( colors, cursor movements, inserting, erasing lines, etc.                     )
( Note: the module uses the pictured output buffer.                            )


1 constant trm.version


( Attributes )

0  constant trm.reset                 ( -- u = Reset attributes to defaults )
1  constant trm.bold                  ( -- u = Set bold )
2  constant trm.half-bright           ( -- u = Set half bright )
4  constant trm.underscore-on         ( -- u = Set underscore )
5  constant trm.blink-on              ( -- u = Set blink )
7  constant trm.reverse-on            ( -- u = Set reverse video )
22 constant trm.normal-intensity      ( -- u = Set normal intensity )
24 constant trm.underline-off         ( -- u = Reset underline )
25 constant trm.blink-off             ( -- u = Reset blink )
27 constant trm.reverse-off           ( -- u = Reset reverse )
30 constant trm.foreground-black      ( -- u = Set black foreground )
31 constant trm.foreground-red        ( -- u = Set red foreground )
32 constant trm.foreground-green      ( -- u = Set green foreground )
33 constant trm.foreground-brown      ( -- u = Set brown foreground )
34 constant trm.foreground-blue       ( -- u = Set blue foreground )
35 constant trm.foreground-magenta    ( -- u = Set magenta foreground )
36 constant trm.foreground-cyan       ( -- u = Set cyan foreground )
37 constant trm.foreground-white      ( -- u = Set white foreground )
38 constant trm.foreground-def-underline ( -- u = Set default foreground with underscore on )
39 constant trm.foreground-default    ( -- u = Set default foreground )
40 constant trm.background-black      ( -- u = Set black background )
41 constant trm.background-red        ( -- u = Set red background )
42 constant trm.background-green      ( -- u = Set green background )
43 constant trm.background-brown      ( -- u = Set brown background )
44 constant trm.background-blue       ( -- u = Set blue background )
45 constant trm.background-magenta    ( -- u = Set magenta background )
46 constant trm.background-cyan       ( -- u = Set cyan background )
47 constant trm.background-white      ( -- u = Set white background )
49 constant trm.background-default    ( -- u = Set default background )


( Private words )

: trm+do-esc1      ( char -- = Output a single escape sequence )
  27 emit
  emit
;


: trm+do-esc2      ( char1 char2 -- = Output a double escape sequence )
  27 emit
  emit
  emit
;


: trm+do-csin      ( u1 .. un n char -- = Output a CSI escape sequence with multiple fields )
  base @ >r decimal
  >r
  27 emit
  [char] [ emit
  BEGIN
    ?dup
  WHILE
    1- swap
    0 u.r
    dup IF
      [char] ; emit
    THEN
  REPEAT
  r> emit
  r> base !
;


: trm+do-csi0      ( char -- = Output a CSI escape sequence with no number fields )
  27 emit
  [char] [ emit
  emit
;  


: trm+do-csi1      ( u char -- = Output a CSI escape sequence with one number field )
  1 swap trm+do-csin
;  


( Terminal words )

: trm+reset        ( -- = Reset the terminal )
  [char] c trm+do-esc1
;


: trm+save-current-state  ( -- = Save the current state: cursor, attributes and character sets )
  [char] 7 trm+do-esc1
;


: trm+restore-current-state  ( -- = Restore the current state: cursor, attributes and character sets )
  [char] 8 trm+do-esc1
;


( Tab words )

: trm+set-tab-stop ( -- = Set tab stop at current column )
  [char] H trm+do-esc1
;


: trm+clear-tab-stop  ( -- = Clear tab stop at current column )
  [char] g trm+do-csi0
;


: trm+clear-all-tab-stops  ( -- = Clear all tab stops )
  3 [char] g trm+do-csi1
;


( Scroll words )

: trm+set-scroll-region  ( u1 u2 -- = Set the scroll region rows with top u2 and bottom u1 )
  2 [char] r trm+do-csin
;


: trm+scroll-up    ( -- = Scroll the display up )
  [char] M trm+do-esc1
;


: trm+scroll-down  ( -- = Scroll the display down )
  [char] D trm+do-esc1
;


( Cursor words )

: trm+move-cursor-up ( u -- = Move cursor up u rows )
  [char] A trm+do-csi1
;


: trm+move-cursor-down ( u -- = Move cursor down u rows )
  [char] B trm+do-csi1
;


: trm+move-cursor-right ( u -- = Move cursor right u columns )
  [char] C trm+do-csi1
;


: trm+move-cursor-left ( u -- = Move cursor left u columns )
  [char] D trm+do-csi1
;


: trm+move-cursor  ( u1 u2 -- = Move cursor to column and row with x u1 and y u2 )
  2 [char] H trm+do-csin
;


: trm+save-cursor  ( -- = Save cursor location )
  [char] s trm+do-csi0
;


: trm+restore-cursor  ( -- = Restore cursor location )
  [char] u trm+do-csi0
;


( Erase display words )

: trm+erase-display-down  ( -- = Erase display from cursor to end )
  [char] J trm+do-csi0
;


: trm+erase-display-up  ( -- = Erase from start display to cursor )
  1 [char] J trm+do-csi1
;


: trm+erase-display  ( -- = Erase the whole display )
  2 [char] J trm+do-csi1
;


( Erase line words )

: trm+erase-end-of-line  ( -- = Erase the line from cursor to end of line )
  [char] K trm+do-csi0
;


: trm+erase-start-of-line  ( -- = Erase the line from start line to cursor )
  1 [char] K trm+do-csi1
;


: trm+erase-line   ( -- = Erase the whole line )
  2 [char] K trm+do-csi1
;


( Insert and delete lines words )

: trm+insert-lines  ( u -- = Insert u blank lines )
  [char] L trm+do-csi1
;


: trm+delete-lines  ( u -- = Delete u lines )
  [char] M trm+do-csi1
;


( Character words )

: trm+insert-spaces  ( u -- = Insert u spaces )
  [char] @ trm+do-csi1
;


: trm+delete-chars  ( u -- = Delete n characters on the current line )
  [char] P trm+do-csi1
;


: trm+erase-chars  ( u -- = Erase u characters on the current line )
  [char] X trm+do-csi1
;


( Attribute words )

: trm+set-attributes  ( u1 .. un n -- = Set n attributes )
  [char] m trm+do-csin
;


( LED words )

: trm+clear-all-leds  ( -- = Clear all LEDs )
  0 [char] q trm+do-csi1
;


: trm+set-scroll-led  ( -- = Set the scroll lock LED )
  1 [char] q trm+do-csi1
;


: trm+set-num-led  ( -- = Set the num lock LED )
  2 [char] q trm+do-csi1
;


: trm+set-caps-led  ( -- = Set the caps lock LED )
  3 [char] q trm+do-csi1
;


( Character set words )

: trm+select-default-font  ( -- = Select the default character set )
  15 emit
;


: trm+select-alternate-font  ( -- = Select the alternate character set )
  14 emit
;


( Linux console words )

: trm+set-default-attributes  ( -- = Set the current attributes the default attributes )
  8 [char] ] trm+do-csi1
;


: trm+set-screen-blank-timeout  ( u -- = Set the screen blank timeout in minutes )
  9 2 [char] ] trm+do-csin
;


: trm+activate-console  ( u -- = Bring the console to the front )
  12 2 [char] ] trm+do-csin
;


: trm+unblank-screen  ( -- = Unblank the screen )
  13 [char] ] trm+do-csi1
;


: trm+select-default  ( -- = Select the default character set ISO8859-1 )
  [char] @ [char] % trm+do-esc2
;


: trm+select-UTF-8  ( -- = Select the UTF-8 character set )
  [char] G [char] % trm+do-esc2
;


: trm+select-graphics-font2 ( -- = Select the vt100 graphics font for the alternate font )
  [char] 0 [char] ) trm+do-esc2
;

[THEN]

\ ==============================================================================
