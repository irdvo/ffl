\ ==============================================================================
\
\           msc_expl - the message catalog example in the ffl
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
\  $Date: 2008-03-04 18:39:16 $ $Revision: 1.2 $
\
\ ==============================================================================

include ffl/msc.fs


\ Example 1 : translation of english to dutch


\ Create the message catalog in the dictionary

msc-create en>nl


\ Add translations to the catalog

s" yes"  s" ja"    en>nl msc-add
s" no"   s" nee"   en>nl msc-add
s" tree" s" boom"  en>nl msc-add
s" bike" s" fiets" en>nl msc-add

\ Translate messages

s" yes"   en>nl msc-translate type cr
s" bike"  en>nl msc-translate type cr
s" house" en>nl msc-translate type cr     \ if not in the catalog, the orignal message is returned
  
  
  
\ Example 2 : entity references in html
  
\ Create the message catalog on the heap

msc-new value entity


\ Add the references in the catalog

s" lt"   s" <"                   entity msc-add
s" gt"   s" >"                   entity msc-add
s" amp"  s" &"                   entity msc-add
s" quot"  char " pad c!  pad 1   entity msc-add


\ Convert the references

s" lt"   entity msc-translate type cr
s" quot" entity msc-translate type cr


\ Free the catalog from the heap

entity msc-free


  

    
