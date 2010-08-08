\ ==============================================================================
\
\         chs_test - the test words for the chs module in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2008-10-04 14:56:56 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/chs.fs
include ffl/tst.fs

.( Testing: chs) cr 
  
chs-create chs1

t{ chs1 chs-set }t

t{ chr.nul chs1 chs-char? ?true }t
t{ char 7  chs1 chs-char? ?true }t
t{ char }  chs1 chs-char? ?true }t
t{ chr.del chs1 chs-char? ?true }t


t{ chs1 chs-reset }t

t{ chr.nul chs1 chs-char? ?false }t
t{ char 7  chs1 chs-char? ?false }t
t{ char a  chs1 chs-char? ?false }t
t{ chr.del chs1 chs-char? ?false }t

\ String words

t{ chs1 chs-reset }t

t{ s" abcefgijk" chs1 chs-set-string }t
t{ char a chs1 chs-char? ?true  }t
t{ char c chs1 chs-char? ?true  }t
t{ char d chs1 chs-char? ?false }t
t{ char k chs1 chs-char? ?true  }t
t{ char l chs1 chs-char? ?false }t

t{ s" ceklm" chs1 chs-reset-string }t
t{ char a chs1 chs-char? ?true  }t
t{ char c chs1 chs-char? ?false  }t
t{ char d chs1 chs-char? ?false }t
t{ char k chs1 chs-char? ?false }t
t{ char l chs1 chs-char? ?false }t
t{ char j chs1 chs-char? ?true  }t


\ List words 

t{ chs1 chs-reset }t

t{ char a char b char c 3 chs1 chs-set-list }t
t{                      0 chs1 chs-set-list }t
t{ char a chs1 chs-char? ?true  }t
t{ char b chs1 chs-char? ?true  }t
t{ char c chs1 chs-char? ?true  }t
t{ char 7 chs1 chs-char? ?false }t

t{ chs1 chs-set }t

t{ char ; bl char _ char = 4 chs1 chs-reset-list }t
t{                         0 chs1 chs-reset-list }t
t{ char ; chs1 chs-char? ?false }t
t{ bl     chs1 chs-char? ?false }t
t{ char _ chs1 chs-char? ?false }t
t{ char = chs1 chs-char? ?false }t
t{ char a chs1 chs-char? ?true  }t


\ Set class words

t{ chs1 chs-reset }t

t{ chs1 chs-set-upper }t

t{ char @ chs1 chs-char? ?false }t
t{ char A chs1 chs-char? ?true  }t
t{ char M chs1 chs-char? ?true  }t
t{ char Z chs1 chs-char? ?true  }t
t{ char [ chs1 chs-char? ?false }t
t{ char a chs1 chs-char? ?false  }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-lower }t

t{ char ` chs1 chs-char? ?false }t
t{ char a chs1 chs-char? ?true  }t
t{ char m chs1 chs-char? ?true  }t
t{ char z chs1 chs-char? ?true  }t
t{ char { chs1 chs-char? ?false }t
t{ char A chs1 chs-char? ?false  }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-alpha }t

t{ char @ chs1 chs-char? ?false }t
t{ char A chs1 chs-char? ?true  }t
t{ char M chs1 chs-char? ?true  }t
t{ char Z chs1 chs-char? ?true  }t
t{ char [ chs1 chs-char? ?false }t
t{ char ` chs1 chs-char? ?false }t
t{ char a chs1 chs-char? ?true  }t
t{ char m chs1 chs-char? ?true  }t
t{ char z chs1 chs-char? ?true  }t
t{ char { chs1 chs-char? ?false }t

t{ chs1 chs-invert }t

t{ char a chs1 chs-char? ?false }t
t{ char 0 chs1 chs-char? ?true  }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-digit }t

t{ char / chs1 chs-char? ?false }t
t{ char 0 chs1 chs-char? ?true  }t
t{ char 3 chs1 chs-char? ?true  }t
t{ char 9 chs1 chs-char? ?true  }t
t{ char : chs1 chs-char? ?false }t
  
  
t{ chs1 chs-reset }t

t{ chs1 chs-set-alnum }t

t{ char @ chs1 chs-char? ?false }t
t{ char A chs1 chs-char? ?true  }t
t{ char M chs1 chs-char? ?true  }t
t{ char Z chs1 chs-char? ?true  }t
t{ char [ chs1 chs-char? ?false }t
t{ char ` chs1 chs-char? ?false }t
t{ char a chs1 chs-char? ?true  }t
t{ char m chs1 chs-char? ?true  }t
t{ char z chs1 chs-char? ?true  }t
t{ char { chs1 chs-char? ?false }t
t{ char / chs1 chs-char? ?false }t
t{ char 0 chs1 chs-char? ?true  }t
t{ char 3 chs1 chs-char? ?true  }t
t{ char 9 chs1 chs-char? ?true  }t
t{ char : chs1 chs-char? ?false }t
t{ char _ chs1 chs-char? ?false }t
  
  
t{ chs1 chs-reset }t

t{ chs1 chs-set-xdigit }t

t{ char @ chs1 chs-char? ?false }t
t{ char A chs1 chs-char? ?true  }t
t{ char F chs1 chs-char? ?true  }t
t{ char G chs1 chs-char? ?false }t
t{ char ` chs1 chs-char? ?false }t
t{ char a chs1 chs-char? ?true  }t
t{ char f chs1 chs-char? ?true  }t
t{ char g chs1 chs-char? ?false }t
t{ char / chs1 chs-char? ?false }t
t{ char 0 chs1 chs-char? ?true  }t
t{ char 9 chs1 chs-char? ?true  }t
t{ char : chs1 chs-char? ?false }t
  
  
t{ chs1 chs-reset }t

t{ chs1 chs-set-punct }t

t{ chr.sp  chs1 chs-char? ?false }t
t{ char !  chs1 chs-char? ?true  }t
t{ char /  chs1 chs-char? ?true  }t
t{ char 0  chs1 chs-char? ?false }t
t{ char 9  chs1 chs-char? ?false }t
t{ char :  chs1 chs-char? ?true  }t
t{ char @  chs1 chs-char? ?true  }t
t{ char A  chs1 chs-char? ?false }t
t{ char Z  chs1 chs-char? ?false }t
t{ char [  chs1 chs-char? ?true  }t
t{ char `  chs1 chs-char? ?true  }t
t{ char a  chs1 chs-char? ?false }t
t{ char z  chs1 chs-char? ?false }t
t{ char {  chs1 chs-char? ?true  }t
t{ char ~  chs1 chs-char? ?true  }t
t{ chr.del chs1 chs-char? ?false }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-blank }t

t{ chr.us  chs1 chs-char? ?false }t
t{ chr.sp  chs1 chs-char? ?true  }t
t{ chr.ht  chs1 chs-char? ?true  }t
t{ chr.vt  chs1 chs-char? ?false }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-space }t

t{ chr.us  chs1 chs-char? ?false }t
t{ chr.sp  chs1 chs-char? ?true  }t
t{ chr.ht  chs1 chs-char? ?true  }t
t{ chr.vt  chs1 chs-char? ?true  }t
t{ chr.lf  chs1 chs-char? ?true  }t
t{ chr.ff  chs1 chs-char? ?true  }t
t{ chr.cr  chs1 chs-char? ?true  }t
t{ char !  chs1 chs-char? ?false }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-cntrl }t

t{ chr.nul chs1 chs-char? ?true  }t
t{ chr.vt  chs1 chs-char? ?true  }t
t{ chr.us  chs1 chs-char? ?true  }t
t{ chr.sp  chs1 chs-char? ?false }t
t{ char ~  chs1 chs-char? ?false }t
t{ chr.del chs1 chs-char? ?true  }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-graph }t

t{ chr.sp  chs1 chs-char? ?false  }t
t{ char !  chs1 chs-char? ?true   }t
t{ char 0  chs1 chs-char? ?true   }t
t{ char :  chs1 chs-char? ?true   }t
t{ char A  chs1 chs-char? ?true   }t
t{ char [  chs1 chs-char? ?true   }t
t{ char a  chs1 chs-char? ?true   }t
t{ char ~  chs1 chs-char? ?true   }t
t{ chr.del chs1 chs-char? ?false  }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-print }t

t{ chr.us  chs1 chs-char? ?false  }t
t{ chr.sp  chs1 chs-char? ?true   }t
t{ char !  chs1 chs-char? ?true   }t
t{ char 0  chs1 chs-char? ?true   }t
t{ char :  chs1 chs-char? ?true   }t
t{ char A  chs1 chs-char? ?true   }t
t{ char [  chs1 chs-char? ?true   }t
t{ char a  chs1 chs-char? ?true   }t
t{ char ~  chs1 chs-char? ?true   }t
t{ chr.del chs1 chs-char? ?false  }t


t{ chs1 chs-reset }t

t{ chs1 chs-set-word }t

t{ char @ chs1 chs-char? ?false }t
t{ char A chs1 chs-char? ?true  }t
t{ char M chs1 chs-char? ?true  }t
t{ char Z chs1 chs-char? ?true  }t
t{ char [ chs1 chs-char? ?false }t
t{ char ` chs1 chs-char? ?false }t
t{ char a chs1 chs-char? ?true  }t
t{ char m chs1 chs-char? ?true  }t
t{ char z chs1 chs-char? ?true  }t
t{ char { chs1 chs-char? ?false }t
t{ char / chs1 chs-char? ?false }t
t{ char 0 chs1 chs-char? ?true  }t
t{ char 3 chs1 chs-char? ?true  }t
t{ char 9 chs1 chs-char? ?true  }t
t{ char : chs1 chs-char? ?false }t
t{ char _ chs1 chs-char? ?true  }t


\ Reset class words

t{ chs1 chs-set }t

t{ chs1 chs-reset-upper }t

t{ char @ chs1 chs-char? ?true  }t
t{ char A chs1 chs-char? ?false }t
t{ char M chs1 chs-char? ?false }t
t{ char Z chs1 chs-char? ?false }t
t{ char [ chs1 chs-char? ?true  }t
t{ char a chs1 chs-char? ?true   }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-lower }t

t{ char ` chs1 chs-char? ?true  }t
t{ char a chs1 chs-char? ?false }t
t{ char m chs1 chs-char? ?false }t
t{ char z chs1 chs-char? ?false }t
t{ char { chs1 chs-char? ?true  }t
t{ char A chs1 chs-char? ?true   }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-alpha }t

t{ char @ chs1 chs-char? ?true  }t
t{ char A chs1 chs-char? ?false }t
t{ char M chs1 chs-char? ?false }t
t{ char Z chs1 chs-char? ?false }t
t{ char [ chs1 chs-char? ?true  }t
t{ char ` chs1 chs-char? ?true  }t
t{ char a chs1 chs-char? ?false }t
t{ char m chs1 chs-char? ?false }t
t{ char z chs1 chs-char? ?false }t
t{ char { chs1 chs-char? ?true  }t

t{ chs1 chs-invert }t

t{ char a chs1 chs-char? ?true  }t
t{ char 0 chs1 chs-char? ?false }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-digit }t

t{ char / chs1 chs-char? ?true  }t
t{ char 0 chs1 chs-char? ?false }t
t{ char 3 chs1 chs-char? ?false }t
t{ char 9 chs1 chs-char? ?false }t
t{ char : chs1 chs-char? ?true  }t
  
  
t{ chs1 chs-set }t

t{ chs1 chs-reset-alnum }t

t{ char @ chs1 chs-char? ?true  }t
t{ char A chs1 chs-char? ?false }t
t{ char M chs1 chs-char? ?false }t
t{ char Z chs1 chs-char? ?false }t
t{ char [ chs1 chs-char? ?true  }t
t{ char ` chs1 chs-char? ?true  }t
t{ char a chs1 chs-char? ?false }t
t{ char m chs1 chs-char? ?false }t
t{ char z chs1 chs-char? ?false }t
t{ char { chs1 chs-char? ?true  }t
t{ char / chs1 chs-char? ?true  }t
t{ char 0 chs1 chs-char? ?false }t
t{ char 3 chs1 chs-char? ?false }t
t{ char 9 chs1 chs-char? ?false }t
t{ char : chs1 chs-char? ?true  }t
t{ char _ chs1 chs-char? ?true  }t
  
  
t{ chs1 chs-set }t

t{ chs1 chs-reset-xdigit }t

t{ char @ chs1 chs-char? ?true  }t
t{ char A chs1 chs-char? ?false }t
t{ char F chs1 chs-char? ?false }t
t{ char G chs1 chs-char? ?true  }t
t{ char ` chs1 chs-char? ?true  }t
t{ char a chs1 chs-char? ?false }t
t{ char f chs1 chs-char? ?false }t
t{ char g chs1 chs-char? ?true  }t
t{ char / chs1 chs-char? ?true  }t
t{ char 0 chs1 chs-char? ?false }t
t{ char 9 chs1 chs-char? ?false }t
t{ char : chs1 chs-char? ?true  }t
  
  
t{ chs1 chs-set }t

t{ chs1 chs-reset-punct }t

t{ chr.sp  chs1 chs-char? ?true  }t
t{ char !  chs1 chs-char? ?false }t
t{ char /  chs1 chs-char? ?false }t
t{ char 0  chs1 chs-char? ?true  }t
t{ char 9  chs1 chs-char? ?true  }t
t{ char :  chs1 chs-char? ?false }t
t{ char @  chs1 chs-char? ?false }t
t{ char A  chs1 chs-char? ?true  }t
t{ char Z  chs1 chs-char? ?true  }t
t{ char [  chs1 chs-char? ?false }t
t{ char `  chs1 chs-char? ?false }t
t{ char a  chs1 chs-char? ?true  }t
t{ char z  chs1 chs-char? ?true  }t
t{ char {  chs1 chs-char? ?false }t
t{ char ~  chs1 chs-char? ?false }t
t{ chr.del chs1 chs-char? ?true  }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-blank }t

t{ chr.us  chs1 chs-char? ?true  }t
t{ chr.sp  chs1 chs-char? ?false }t
t{ chr.ht  chs1 chs-char? ?false }t
t{ chr.vt  chs1 chs-char? ?true  }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-space }t

t{ chr.us  chs1 chs-char? ?true  }t
t{ chr.sp  chs1 chs-char? ?false }t
t{ chr.ht  chs1 chs-char? ?false }t
t{ chr.vt  chs1 chs-char? ?false }t
t{ chr.lf  chs1 chs-char? ?false }t
t{ chr.ff  chs1 chs-char? ?false }t
t{ chr.cr  chs1 chs-char? ?false }t
t{ char !  chs1 chs-char? ?true  }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-cntrl }t

t{ chr.nul chs1 chs-char? ?false }t
t{ chr.vt  chs1 chs-char? ?false }t
t{ chr.us  chs1 chs-char? ?false }t
t{ chr.sp  chs1 chs-char? ?true  }t
t{ char ~  chs1 chs-char? ?true  }t
t{ chr.del chs1 chs-char? ?false }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-graph }t

t{ chr.sp  chs1 chs-char? ?true   }t
t{ char !  chs1 chs-char? ?false  }t
t{ char 0  chs1 chs-char? ?false  }t
t{ char :  chs1 chs-char? ?false  }t
t{ char A  chs1 chs-char? ?false  }t
t{ char [  chs1 chs-char? ?false  }t
t{ char a  chs1 chs-char? ?false  }t
t{ char ~  chs1 chs-char? ?false  }t
t{ chr.del chs1 chs-char? ?true   }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-print }t

t{ chr.us  chs1 chs-char? ?true   }t
t{ chr.sp  chs1 chs-char? ?false  }t
t{ char !  chs1 chs-char? ?false  }t
t{ char 0  chs1 chs-char? ?false  }t
t{ char :  chs1 chs-char? ?false  }t
t{ char A  chs1 chs-char? ?false  }t
t{ char [  chs1 chs-char? ?false  }t
t{ char a  chs1 chs-char? ?false  }t
t{ char ~  chs1 chs-char? ?false  }t
t{ chr.del chs1 chs-char? ?true   }t


t{ chs1 chs-set }t

t{ chs1 chs-reset-word }t

t{ char @ chs1 chs-char? ?true  }t
t{ char A chs1 chs-char? ?false }t
t{ char M chs1 chs-char? ?false }t
t{ char Z chs1 chs-char? ?false }t
t{ char [ chs1 chs-char? ?true  }t
t{ char ` chs1 chs-char? ?true  }t
t{ char a chs1 chs-char? ?false }t
t{ char m chs1 chs-char? ?false }t
t{ char z chs1 chs-char? ?false }t
t{ char { chs1 chs-char? ?true  }t
t{ char / chs1 chs-char? ?true  }t
t{ char 0 chs1 chs-char? ?false }t
t{ char 3 chs1 chs-char? ?false }t
t{ char 9 chs1 chs-char? ?false }t
t{ char : chs1 chs-char? ?true  }t
t{ char _ chs1 chs-char? ?false }t


\ execute and move word

t{ chs-new value chs2 }t

t{ chs1 chs-reset }t

t{ chs1 chs-set-alnum }t

t{ chs2 chs-set-punct }t

t{ chs1 chs2 chs^move }t

: chs-test-execute  ( n c - n+1 = Test execute )
  drop 1+
;

t{ 0 ' chs-test-execute chs2 chs-execute 62 ?s }t   \ char set count alnum

: chs-test-execute2 ( n c - n+1 flag = Test execute? )
  drop 1+ dup 51 =
;

t{ 0 ' chs-test-execute2 chs2 chs-execute? ?true 51 ?s }t

: chs-test-execute3 ( n c - n+1 flag = Test execute? )
  drop 1+ dup 71 =
;

t{ 0 ' chs-test-execute3 chs2 chs-execute? ?false 62 ?s }t

\ operator words 

t{ chs1 chs-reset }t
t{ chs2 chs-reset }t

t{ chs1 chs-set-digit }t
t{ chs2 chs-set-space }t

t{ chs2 chs1 chs^or }t

t{ 0 ' chs-test-execute chs1 chs-execute 16 ?s }t

t{ char 8 chs1 chs-char? ?true  }t
t{ chr.vt chs1 chs-char? ?true  }t
t{ char a chs1 chs-char? ?false }t


t{ chs1 chs-reset }t
t{ chs2 chs-reset }t

t{ chs1 chs-set-xdigit }t
t{ chs2 chs-set-digit  }t

t{ chs2 chs1 chs^and }t

t{ 0 ' chs-test-execute chs1 chs-execute 10 ?s }t

t{ char 5 chs1 chs-char? ?true  }t
t{ char a chs1 chs-char? ?false }t


t{ chs1 chs-reset }t
t{ chs2 chs-reset }t

t{ chs1 chs-set-xdigit }t
t{ chs2 chs-set-digit  }t

t{ chs2 chs1 chs^xor }t

t{ 0 ' chs-test-execute chs1 chs-execute 12 ?s }t

t{ char 5 chs1 chs-char? ?false  }t
t{ char a chs1 chs-char? ?true }t

t{ chs2 chs-free }t

\ ==============================================================================
