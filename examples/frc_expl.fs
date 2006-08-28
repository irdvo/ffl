\ ==============================================================================
\
\         frc_expl - the example file for the frc module in the ffl
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
\  $Date: 2006-08-28 17:45:39 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/frc.fs                     \ include frc.fs for using fractions

1 2 1 3 frc+add                        \ 1/2 + 1/3 
1 6     frc+subtract                   \ - 1/6
1 2     frc+multiply                   \ * 1/2
3 5     frc+divide                     \ / 3/5
        frc+invert                     \ invert
        frc+negate                     \ negate
        frc+to-string type cr          \ convert to string and type: -9/5
          
          
 1 2 1 3 frc+compare . cr              \ compare 1/2 and 1/3 :  1 (= bigger)
-2 3 1 3 frc+compare . cr              \ compare -2/3 and 1/3: -1 (= smaller)
 3 9 1 3 frc+compare . cr              \ compare 3/9 and 1/3 :  0 (= equal)    
   
   
   
frc-create frc1                        \ create in the dictionary a fraction

1 2 frc1 frc-set                       \ set 1/2 in the fraction

frc1 frc-num@ .  cr                    \ get the numerator (1)
frc1 frc-denom@ . cr                   \ get the denomerator (2)

1 3  frc1 frc-get  frc+add             \ add 1/3 to the contents of the fraction ..
frc1 frc-set                           \ .. and store the result in the fraction

[DEFINED] frc+to-float

frc1 frc-get frc+to-float f. cr        \ convert to a float: 0.833333

[THEN]


frc-new value frc2                     \ create on the heap a fraction

frc1 frc2 frc^move                     \ move fraction1 in fraction 2

frc2 frc-get . .  cr                   \ get the numerator and the denomerator (6 and 5)

frc2 frc1 frc^compare . cr             \ compare the two fractions: equal (0)
  
frc2 frc-free                          \ done, free the fraction from the heap

\ ==============================================================================
