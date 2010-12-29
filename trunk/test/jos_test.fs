\ ==============================================================================
\
\        jos_test - the test words for the jos module in the ffl
\
\               Copyright (C) 2010  Dick van Oudheusden
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
\  $Date: 2008-03-02 15:03:03 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/jos.fs
include ffl/tst.fs
include ffl/est.fs


.( Testing: jos) cr

t{ jos-create jos1 }t

t{                     jos1 jos-write-start-object }t
t{ s" value"           jos1 jos-write-name         }t
t{ 10                  jos1 jos-write-number       }t
t{ s" flag"            jos1 jos-write-name         }t
t{ false               jos1 jos-write-boolean      }t
t{ s" array"           jos1 jos-write-name         }t
t{                     jos1 jos-write-start-array  }t
t{ 1                   jos1 jos-write-number       }t
t{ 2                   jos1 jos-write-number       }t
t{ 3                   jos1 jos-write-number       }t
t{                     jos1 jos-write-end-array    }t
t{ s" string"          jos1 jos-write-name         }t
t{ s" hello"           jos1 jos-write-string       }t
t{ s" empty"           jos1 jos-write-name         }t
t{ s" "                jos1 jos-write-string       }t
t{                     jos1 jos-write-end-object   }t

t{ s\" {\"value\":10,\"flag\":false,\"array\":[1,2,3],\"string\":\"hello\",\"empty\":\"\"}" jos1 str-get ?str }t

t{ jos-new value jos2 }t

t{                     jos2 jos-write-start-object  }t
t{ s" first"           jos2 jos-write-name          }t
t{                     jos2 jos-write-start-object  }t
t{ s" second"          jos2 jos-write-name          }t
t{                     jos2 jos-write-start-object  }t
t{ s" third"           jos2 jos-write-name          }t
t{                     jos2 jos-write-start-object  }t
t{ s\" special/"       jos2 jos-write-name          }t
t{ s\" \"\n\b\f\r\\\t" jos2 jos-write-string        }t
t{                     jos2 jos-write-end-object    }t
t{ s" value"           jos2 jos-write-name          }t
t{ 2                   jos2 jos-write-number        }t
t{                     jos2 jos-write-end-object    }t
t{ s" value"           jos2 jos-write-name          }t
t{ 3                   jos2 jos-write-number        }t
t{                     jos2 jos-write-end-object    }t
t{                     jos2 jos-write-end-object    }t

t{ s\" {\"first\":{\"second\":{\"third\":{\"special\\/\":\"\\\"\\n\\b\\f\\r\\\\\\t\"},\"value\":2},\"value\":3}}" jos2 str-get ?str }t

jos2 tos-rewrite

t{                     jos2 jos-write-start-array   }t
t{                     jos2 jos-write-start-object  }t
t{ s" first"           jos2 jos-write-name          }t
t{ 1                   jos2 jos-write-number        }t
t{                     jos2 jos-write-end-object    }t
t{                     jos2 jos-write-start-array   }t
t{                     jos2 jos-write-end-array     }t
t{                     jos2 jos-write-start-object  }t
t{ s" second"          jos2 jos-write-name          }t
t{ 2                   jos2 jos-write-number        }t
t{                     jos2 jos-write-end-object    }t
t{                     jos2 jos-write-end-array     }t

t{ s\" [{\"first\":1},[],{\"second\":2}]" jos2 str-get ?str }t

t{ jos2 jos-free }t

\ ==============================================================================
