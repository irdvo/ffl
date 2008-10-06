\ ==============================================================================
\
\     rgx_test - the test words for the rgx,nfe and nfs module in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2008-10-06 18:22:09 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/rgx.fs
include ffl/tst.fs


.( Testing: rgx, nfe and nfs) cr 

t{ rgx-create rgx1 }t

\ Pattern scanner errors

t{ s" a(" rgx1 rgx-compile ?false 1 ?s }t
t{ s" a)" rgx1 rgx-compile ?false 1 ?s }t
t{ s" ()" rgx1 rgx-compile ?false 0 ?s }t
t{ s" a|" rgx1 rgx-compile ?false 2 ?s }t
t{ s" |a" rgx1 rgx-compile ?false 0 ?s }t
t{ s" *"  rgx1 rgx-compile ?false 0 ?s }t
t{ s" +"  rgx1 rgx-compile ?false 0 ?s }t
t{ s" ?"  rgx1 rgx-compile ?false 0 ?s }t
t{ s" ["  rgx1 rgx-compile ?false 0 ?s }t
t{ s" []" rgx1 rgx-compile ?false 0 ?s }t
t{ s" [a" rgx1 rgx-compile ?false 0 ?s }t
t{ s" a[" rgx1 rgx-compile ?false 1 ?s }t

\ Correct patterns

t{ s" ((a)(b))" rgx1 rgx-compile ?true }t

t{ s" ab" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result 0 ?s 2 ?s }t
t{ 1 rgx1 rgx-result 0 ?s 2 ?s }t
t{ 2 rgx1 rgx-result 0 ?s 1 ?s }t
t{ 3 rgx1 rgx-result 1 ?s 2 ?s }t


t{ s" (a)*" rgx1 rgx-compile ?true }t

t{ s" aa" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result 0 ?s 2 ?s }t
t{ 1 rgx1 rgx-result 1 ?s 2 ?s }t


t{ s" (a)*b" rgx1 rgx-compile ?true }t

t{ s" b" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result  0 ?s  1 ?s }t
t{ 1 rgx1 rgx-result -1 ?s -1 ?s }t


t{ s" (a*)b" rgx1 rgx-compile ?true }t

t{ s" b" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result  0 ?s  1 ?s }t
t{ 1 rgx1 rgx-result  0 ?s  0 ?s }t


t{ s" ((a*)b)*" rgx1 rgx-compile ?true }t

t{ s" abb" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result  0 ?s  3 ?s }t
t{ 1 rgx1 rgx-result  2 ?s  3 ?s }t
t{ 2 rgx1 rgx-result  2 ?s  2 ?s }t


t{ s" ((a)*b)*" rgx1 rgx-compile ?true }t

t{ s" abb" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result  0 ?s  3 ?s }t
t{ 1 rgx1 rgx-result  2 ?s  3 ?s }t
t{ 2 rgx1 rgx-result  0 ?s  1 ?s }t


t{ s" ((a)*b)*c" rgx1 rgx-compile ?true }t

t{ s" c" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result   0 ?s   1 ?s }t
t{ 1 rgx1 rgx-result  -1 ?s  -1 ?s }t
t{ 2 rgx1 rgx-result  -1 ?s  -1 ?s }t


t{ s" (a*)+" rgx1 rgx-compile ?true }t

t{ s" aaa" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result 0 ?s 3 ?s }t
t{ 1 rgx1 rgx-result 0 ?s 3 ?s }t


t{ s" (a|aa)(a|aa)" rgx1 rgx-compile ?true }t

t{ s" aaa" rgx1 rgx-cmatch? ?true }t

t{ 0 rgx1 rgx-result 0 ?s 2 ?s }t
t{ 1 rgx1 rgx-result 0 ?s 1 ?s }t
t{ 2 rgx1 rgx-result 1 ?s 2 ?s }t


t{ s" .*(Hello|Bye)" rgx1 rgx-compile ?true }t

t{ s" This is then goodbye" rgx1 rgx-cmatch? ?false }t
t{ s" This is then goodbye" rgx1 rgx-imatch? ?true  }t


t{ s" [abc]" rgx1 rgx-compile ?true }t

t{ s" a" rgx1 rgx-cmatch? ?true  }t
t{ s" z" rgx1 rgx-cmatch? ?false }t


t{ s" [^abc]" rgx1 rgx-compile ?true }t

t{ s" a" rgx1 rgx-cmatch? ?false }t
t{ s" z" rgx1 rgx-cmatch? ?true  }t


t{ s" [a-y]" rgx1 rgx-compile ?true }t

t{ s" a" rgx1 rgx-cmatch? ?true  }t
t{ s" y" rgx1 rgx-cmatch? ?true  }t
t{ s" z" rgx1 rgx-cmatch? ?false }t


t{ s" [\w]" rgx1 rgx-compile ?true }t

t{ s" a" rgx1 rgx-cmatch? ?true  }t
t{ s" ;" rgx1 rgx-cmatch? ?false }t


t{ s" [\d]" rgx1 rgx-compile ?true }t

t{ s" 7" rgx1 rgx-cmatch? ?true  }t
t{ s" p" rgx1 rgx-cmatch? ?false }t

t{ s" [\s]" rgx1 rgx-compile ?true }t

t{ s"  " rgx1 rgx-cmatch? ?true  }t
t{ s" ." rgx1 rgx-cmatch? ?false }t


t{ s" []a]" rgx1 rgx-compile ?true }t

t{ s" ]" rgx1 rgx-cmatch? ?true  }t
t{ s" a" rgx1 rgx-cmatch? ?true  }t
t{ s" 0" rgx1 rgx-cmatch? ?false }t

t{ s" ab[cde-g\d]+z" rgx1 rgx-compile ?true }t

t{ s" abcdefg9876543210z" rgx1 rgx-cmatch? ?true }t

t{ s" abz" rgx1 rgx-cmatch? ?false }t


t{ rgx-new value rgx2 }t

t{ s" \*\|\\" rgx2 rgx-compile ?true }t

t{ s" *|\"    rgx2 rgx-cmatch? ?true  }t
t{ s" \*\|\\" rgx2 rgx-cmatch? ?false }t


t{ s" \w\W\d\D\s\S" rgx2 rgx-compile ?true }t

t{ s" a+0a a" rgx2 rgx-cmatch? ?true  }t
t{ s" _=9! @" rgx2 rgx-cmatch? ?true  }t
t{ s" 0aa0y " rgx2 rgx-cmatch? ?false }t


t{ s" good|Bad" rgx2 rgx-compile ?true }t

t{ s" This is a good day" rgx2 rgx-csearch 10 ?s }t
t{ s" This is a bad day"  rgx2 rgx-csearch -1 ?s }t
t{ s" This is a bad day"  rgx2 rgx-isearch 10 ?s }t
t{ s" This is a good day" rgx2 rgx-isearch 10 ?s }t


t{ s" (\+|-|\s)?\d+(\.\d+)?" rgx2 rgx-compile ?true }t     \ Float number

t{ s" -0.7"         rgx2 rgx-cmatch? ?true  }t
t{ s" +4"           rgx2 rgx-cmatch? ?true  }t
t{ s" 400000000000" rgx2 rgx-cmatch? ?true  }t
t{ s"  12.47"       rgx2 rgx-cmatch? ?true  }t
t{ s" ^1.7"         rgx2 rgx-cmatch? ?false }t
t{ s" +.4"          rgx2 rgx-cmatch? ?false }t

t{ s" The price is 23.70 euro" rgx2 rgx-csearch 12 ?s }t
t{ s" It is priceless"         rgx2 rgx-csearch -1 ?s }t

t{ rgx2 rgx-free }t

\ ==============================================================================
