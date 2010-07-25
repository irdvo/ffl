\ ==============================================================================
\
\          xos_expl - the xml output stream example in the ffl
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
\  $Date: 2008-01-13 08:09:33 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/xos.fs


\ Example 1: Write xml output


\ Create the xml output stream

tos-create xos1

s" encoding" s" ISO-8859-1" s" version" s" 1.0" 2 xos1 xos-write-start-xml

s"  menu of the day " xos1 xos-write-comment

0 s" menu"         xos1 xos-write-start-tag
0 s" food"         xos1 xos-write-start-tag
0 s" name"         xos1 xos-write-start-tag
  s" Breakfast"    xos1 xos-write-text
  s" name"         xos1 xos-write-end-tag
0 s" description"  xos1 xos-write-start-tag
  s" two eggs & bacon or sausage & toast & hash browns" xos1 xos-write-text
  s" description"  xos1 xos-write-end-tag
0 s" price"        xos1 xos-write-start-tag
  s" $6.95"        xos1 xos-write-text
  s" price"        xos1 xos-write-end-tag
s" cholesterol" s" yes"
1 s" health"       xos1 xos-write-empty-element
  s" food"         xos1 xos-write-end-tag
  s" menu"         xos1 xos-write-end-tag


xos1 str-get type cr                 \ Type the xml info


\ Example 2: Write html output


xos1 tos-rewrite                     \ Clean the output stream

nil 0 s" -//W3C//DTD HTML 4.0 Transitional//EN" nil 0 s" HTML" xos1 xos-write-public-dtd

0 s" HTML"         xos1 xos-write-start-tag
0 s" HEAD"         xos1 xos-write-start-tag
0 s" TITLE"        xos1 xos-write-start-tag
  s" HTML example" xos1 xos-write-text
  s" TITLE"        xos1 xos-write-end-tag
0 s" BODY"         xos1 xos-write-start-tag
0 s" H1"           xos1 xos-write-start-tag
  s" 'Welcome'"    xos1 xos-write-text          \ Translate ' to &apos;
  s" H1"           xos1 xos-write-end-tag
  s"  comment "    xos1 xos-write-comment
  
  s" align" s" center"
1 s" DIV"          xos1 xos-write-start-tag     \ = <DIV align="center">

  s" Test"         xos1 xos-write-text
  s" DIV"          xos1 xos-write-end-tag
  s" BODY"         xos1 xos-write-end-tag
  s" HTML"         xos1 xos-write-end-tag
  
xos1 str-get type cr
