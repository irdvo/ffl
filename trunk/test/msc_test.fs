\ ==============================================================================
\
\        arg_test - the test words for the arg module in the ffl
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
\  $Date: 2007-11-12 07:01:48 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/msc.fs
include ffl/tst.fs


.( Testing: msc) cr

t{ msc-new value msc1 }t

\ Add translations

t{ s" yes"  s" ja"   msc1 msc-add }t
t{ s" no"   s" nee"  msc1 msc-add }t
t{ s" tree" s" boom" msc1 msc-add }t

\ Translate

t{ s" no"   msc1 msc-translate s" nee"  compare ?0 }t
t{ s" tree" msc1 msc-translate s" boom" compare ?0 }t
t{ s" bike" msc1 msc-translate s" bike" compare ?0 }t


\ Update translations 

t{ s" no"   s" neen"  msc1 msc-add }t

t{ s" no"   msc1 msc-translate s" neen"  compare ?0 }t

t{ s" no"   s" nee"  msc1 msc-add }t

t{ s" no"   msc1 msc-translate s" nee"   compare ?0 }t


\ Remove translations

t{ s" yes"  msc1 msc-remove ?true  }t
t{ s" tree" msc1 msc-remove ?true  }t
t{ s" bike" msc1 msc-remove ?false }t

t{ s" tree" msc1 msc-translate s" tree" compare ?0 }t

\ Free message catalog

t{ msc1 msc-free }t

\ ==============================================================================
