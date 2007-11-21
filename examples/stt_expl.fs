\ ==============================================================================
\
\             stt_expl - the stringtable example in the ffl
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
\  $Date: 2007-11-21 18:29:11 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/stt.fs


\ Example : the month names in a stringtable


\ Create the stringtable in the dictionary

begin-stringtable months
," January"
," February"
," March"
," April"
," May"
," June"
," July"
," August"
," September"
," October"
," November"
," December"
end-stringtable

\ Fetch strings from the stringtable

0  months type cr       \ January
11 months type cr       \ December



  

    
