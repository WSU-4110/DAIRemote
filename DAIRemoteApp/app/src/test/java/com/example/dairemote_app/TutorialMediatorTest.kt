package com.example.dairemote_app

import androidx.appcompat.app.AlertDialog
import com.example.dairemote_app.fragments.ServersFragment
import com.example.dairemote_app.utils.TutorialMediator
import com.example.dairemote_app.utils.TutorialMediator.Companion.GetInstance
import com.example.dairemote_app.utils.TutorialMediator.Companion.SetBuilder
import org.junit.jupiter.api.AfterEach
import org.junit.jupiter.api.Assertions
import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.Test
import org.mockito.Mockito

internal class TutorialMediatorTest {
    var tutorialMediator: TutorialMediator? = null
    var mockBuilder: AlertDialog.Builder? = null

    @BeforeEach
    fun setUp() {
        tutorialMediator = TutorialMediator()
        mockBuilder = Mockito.mock(AlertDialog.Builder::class.java)
        SetBuilder(mockBuilder)
    }

    @AfterEach
    fun tearDown() {
        tutorialMediator = null
    }

    @get:Test
    val instance: Unit
        get() {
            val instance1 = GetInstance(mockBuilder)
            val instance2 = GetInstance(mockBuilder)

            Assertions.assertNotNull(instance1)
            Assertions.assertSame(instance1, instance2)
        }

    @get:Test
    val builder: Unit
        get() {
            val result = tutorialMediator!!.getBuilder()
            Assertions.assertNotNull(result)
            Assertions.assertSame(mockBuilder, result)
        }


    @get:Test
    val tutorialOn: Unit
        get() {
            tutorialMediator!!.tutorialOn = true
            Assertions.assertTrue(tutorialMediator!!.tutorialOn, "Tutorial is on")

            tutorialMediator!!.tutorialOn = false
            Assertions.assertFalse(tutorialMediator!!.tutorialOn)
        }

    @get:Test
    val currentStep: Unit
        get() {
            tutorialMediator!!.currentStep = 0
            Assertions.assertEquals(0, tutorialMediator!!.currentStep)

            tutorialMediator!!.currentStep = 1
            Assertions.assertEquals(1, tutorialMediator!!.currentStep)

            tutorialMediator!!.currentStep = 2
            Assertions.assertEquals(2, tutorialMediator!!.currentStep)

            tutorialMediator!!.currentStep = 3
            Assertions.assertEquals(3, tutorialMediator!!.currentStep)

            tutorialMediator!!.currentStep = 4
            Assertions.assertEquals(4, tutorialMediator!!.currentStep)

            tutorialMediator!!.currentStep = 5
            Assertions.assertEquals(5, tutorialMediator!!.currentStep)

            tutorialMediator!!.currentStep = 6
            Assertions.assertEquals(6, tutorialMediator!!.currentStep)

            tutorialMediator!!.currentStep = 7
            Assertions.assertEquals(7, tutorialMediator!!.currentStep)
        }

    @Test
    fun setServersPage() {
        val mockServersFragment = Mockito.mock(ServersFragment::class.java)
        tutorialMediator!!.setServersPage(mockServersFragment)
        Assertions.assertNotNull(mockServersFragment)
    }

    @Test
    fun checkIfStepCompleted() {
        tutorialMediator!!.currentStep = 0
        Assertions.assertTrue(tutorialMediator!!.checkIfStepCompleted())

        tutorialMediator!!.currentStep = 3
        Assertions.assertFalse(tutorialMediator!!.checkIfStepCompleted())

        tutorialMediator!!.currentStep = 5
        Assertions.assertTrue(tutorialMediator!!.checkIfStepCompleted())
    }
}