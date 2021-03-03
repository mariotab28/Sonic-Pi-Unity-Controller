'''
==================================
    STRUCTS
==================================
'''

# * Definir un Struct
Command = Struct.new(:loop_id, :com_name) do
    def getName
        "Comando #{com_name}"
    end
end

# * Crear una variable de tipo struct
com = Command.new(0, "play")
# * Llamada a metodos
puts com.metodo 
# Output: 
#   Comando play

# * Comparador ==
# Create structure 
Command = Struct.new(:loop_id, :com_name) 

# Creating objects 
str = Command.new(0, "play") 
other_struct = Command.new(1, "sleep") 
str1 = Command.new(0, "play") 

# Check equality 
p str == other_struct # Output: false
p str == str1 # Output: true

# * Acceso al valor de los atributos 
# Create structure 
Command = Struct.new(:loop_id, :com_name) 

# Creating objects 
str = Command.new(0, "play") 

# Using [] 
p str[:loop_id]    # out: 0
p str["com_name"]  # out: "play"

# * Asignar valor a los atributos 
# Create structure 
Command = Struct.new(:loop_id, :com_name) 

# Creating objects 
str = Command.new(0, "play") 

# Using [] = 
str[:loop_id] = 1
str[:com_name] = "sleep"